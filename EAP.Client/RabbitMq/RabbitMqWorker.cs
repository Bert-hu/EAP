using EAP.Client.Secs;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Secs4Net;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace EAP.Client.RabbitMq
{
    internal class RabbitMqWorker : BackgroundService
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly ILog eqpLog = LogManager.GetLogger("Secs");
        private readonly ILog traLog = LogManager.GetLogger("Trace");

        private readonly IConfiguration _configuration;
        private readonly ISecsConnection _hsmsConnection;
        private readonly ISecsGem _secsGem;
        private CommonLibrary _commonLibrary;
        private RabbitMqService _rabbitMqService;
        private readonly IServiceProvider _serviceProvider;

        private readonly System.Threading.Timer _equipmentStatusTimer;


        public RabbitMqWorker(IConfiguration configuration, ISecsConnection hsmsConnection, ISecsGem secsGem, CommonLibrary commonLibrary, RabbitMqService rabbitMqService,IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _secsGem = secsGem;
            _hsmsConnection = hsmsConnection;
            _commonLibrary = commonLibrary;
            _rabbitMqService = rabbitMqService;
            _serviceProvider = serviceProvider;

            var interval = int.Parse(_configuration.GetSection("RabbitMQ")["GetEquipmentStatusInterval"] ?? "120");

            if (interval > 0)
            {
                _equipmentStatusTimer = new System.Threading.Timer(delegate
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var handler = (ITransactionHandler)scope.ServiceProvider.GetRequiredService(typeof(GetEquipmentStatus));
                        handler.HandleTransaction(null);
                    }
                }, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(interval));
            }
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_rabbitMqService.channel);
            consumer.ReceivedAsync += (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());

                // Process the received message
                dbgLog.Info("RabbitMqService Received message: " + message);

                return Task.Run(() => HandleRecivedTrans(message));
            };
            Dictionary<string, object> arguments = new Dictionary<string, object>() { { "x-message-ttl", 300000 } };
            var queue = _configuration.GetSection("RabbitMQ")["QueueName"];
            try
            {
                _rabbitMqService.channel.QueueDeclareAsync(queue, false, false, true, arguments);
            }
            catch (Exception)
            {
                dbgLog.Warn($"Declare Queue fail： {queue}");
            }
            _rabbitMqService.channel.BasicConsumeAsync(queue: queue, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }



        private void HandleRecivedTrans(string message)
        {
            try
            {
                var trans = JsonConvert.DeserializeObject<RabbitMqTransaction>(message);
                if (trans.IsReply)
                {
                    var tid = trans.TransactionID;
                    if (_rabbitMqService.waitTransactions.TryGetValue(tid, out TaskCompletionSource<RabbitMqTransaction> tcs))
                    {
                        tcs.SetResult(trans);
                    }
                }
                else
                {
                    var interfaceType = typeof(ITransactionHandler);
                    var type = Assembly.GetExecutingAssembly().GetTypes().Where(t => interfaceType.IsAssignableFrom(t) && t.Name == trans.TransactionName).FirstOrDefault();

                    if (type != null && typeof(ITransactionHandler).IsAssignableFrom(type))
                    {
                        using (var scope = _serviceProvider.CreateAsyncScope())
                        {
                            var handler = (ITransactionHandler)scope.ServiceProvider.GetRequiredService(type);
                            handler.HandleTransaction(trans);
                        }
                        //ITransactionHandler obj = (ITransactionHandler)Activator.CreateInstance(type);
                        //obj.HandleTransaction(trans, _rabbitMqService, _secsGem, _hsmsConnection, commonLibrary);
                    }
                    else
                    {
                        dbgLog.Error($"Transaction '{trans.TransactionName}' does not implement ITransactionHandler.");
                        if (trans.NeedReply)
                        {
                            var reptrans = trans?.GetReplyTransaction();
                            reptrans?.Parameters.Add("Result", false);
                            reptrans?.Parameters.Add("Message", $"Transaction '{trans.TransactionName}' is not supported.");
                            _rabbitMqService.Produce(trans.ReplyChannel, reptrans);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            traLog.Info("RabbitMqService Start");
            return base.StartAsync(cancellationToken);
        }
    }
}
