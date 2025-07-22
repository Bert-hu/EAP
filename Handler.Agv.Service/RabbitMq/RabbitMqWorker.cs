using log4net;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;

namespace HandlerAgv.Service.RabbitMq
{
    internal class RabbitMqWorker : BackgroundService
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly ILog eqpLog = LogManager.GetLogger("Secs");
        private readonly ILog traLog = LogManager.GetLogger("Trace");

        private readonly IConfiguration configuration;
        private RabbitMqService rabbitMqService;
        private readonly IServiceProvider serviceProvider;

        private readonly System.Threading.Timer _equipmentStatusTimer;

        public RabbitMqWorker(IConfiguration configuration, RabbitMqService rabbitMqService, IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.rabbitMqService = rabbitMqService;
            this.serviceProvider = serviceProvider;

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(rabbitMqService.channel);
            consumer.ReceivedAsync += (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());

                // Process the received message
                dbgLog.Info("RabbitMqService Received message: " + message);

                return Task.Run(() => HandleRecivedTrans(message));
            };
            Dictionary<string, object> arguments = new Dictionary<string, object>() { { "x-message-ttl", 300000 } };


            rabbitMqService.channel?.QueueDeclareAsync(rabbitMqService.consumeQueue, true, false, true, arguments);
            rabbitMqService.channel?.BasicConsumeAsync(rabbitMqService.consumeQueue, autoAck: true, consumer: consumer);

            rabbitMqService.channel?.QueueDeclareAsync(rabbitMqService.consumeSubQueue, true, false, true, arguments);
            rabbitMqService.channel?.BasicConsumeAsync(rabbitMqService.consumeSubQueue, autoAck: true, consumer: consumer);

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
                    if (rabbitMqService.waitTransactions.TryGetValue(tid, out TaskCompletionSource<RabbitMqTransaction> tcs))
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
                        using (var scope = serviceProvider.CreateAsyncScope())
                        {
                            var handler = (ITransactionHandler)scope.ServiceProvider.GetRequiredService(type);
                            handler.HandleTransaction(trans);
                        }
                        //ITransactionHandler obj = (ITransactionHandler)Activator.CreateInstance(type);
                        //obj.HandleTransaction(trans, rabbitMqService, _secsGem, _hsmsConnection, commonLibrary);
                    }
                    else
                    {
                        dbgLog.Error($"Transaction '{trans.TransactionName}' does not implement ITransactionHandler.");
                        if (trans.NeedReply)
                        {
                            var reptrans = trans?.GetReplyTransaction();
                            reptrans?.Parameters.Add("Result", false);
                            reptrans?.Parameters.Add("Message", $"Transaction '{trans.TransactionName}' is not supported.");
                            rabbitMqService.Produce(trans.ReplyChannel, reptrans);
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
