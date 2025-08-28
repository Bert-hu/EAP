using log4net;
using Newtonsoft.Json;
using RabbitMQ.Client;

using System.Collections.Concurrent;
using System.Text;

namespace HandlerAgv.Service.RabbitMq
{
    public class RabbitMqService


    {
        private readonly IConfiguration configuration;

        ConnectionFactory factory;
        public IConnection connection;
        public IChannel channel;
        internal static ILog dbgLog = LogManager.GetLogger("Debug");
        internal static ILog rabbitMqLog = LogManager.GetLogger("RabbitMq");

        public readonly string consumeQueue;
        public readonly string consumeSubQueue;

        public RabbitMqService
            (IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;

            factory = new ConnectionFactory();
            factory.HostName = configuration.GetSection("RabbitMQ")["HostName"];
            factory.Port = int.Parse(configuration.GetSection("RabbitMQ")["Port"] ?? "5672");
            factory.UserName = configuration.GetSection("RabbitMQ")["UserName"];
            factory.Password = configuration.GetSection("RabbitMQ")["Password"];
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            factory.RequestedHeartbeat = TimeSpan.FromSeconds(10);

            if (!env.IsDevelopment())//正式环境
            {
                consumeQueue = configuration.GetSection("RabbitMQ")["QueueName"];
                consumeSubQueue = consumeQueue + "." + Guid.NewGuid().ToString("N");
                connection = factory.CreateConnectionAsync().Result;
                channel = connection.CreateChannelAsync().Result;
            }
            else //测试环境
            {
                consumeQueue = configuration.GetSection("RabbitMQ")["QueueName"] + ".Test";
                consumeSubQueue = consumeQueue + "." + Guid.NewGuid().ToString("N");
            }

   

        }
        public void Produce(string routingKey, RabbitMqTransaction trans)
        {
            try
            {
                string repchannel = configuration.GetSection("RabbitMQ")["QueueName"];
                if (trans.NeedReply == true && string.IsNullOrEmpty(trans.ReplyChannel)) trans.ReplyChannel = repchannel;
                trans.EquipmentID = trans.EquipmentID ?? configuration.GetSection("Custom")["EquipmentId"];
                var message = JsonConvert.SerializeObject(trans);
                Produce(routingKey, message);
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.Message, ex);
            }
        }
        public void Produce(string routingKey, string message, int ExpireSecond = 60)
        {
            try
            {
                rabbitMqLog.Info($"RabbitMqService Send message to {routingKey}: " + message);
                Dictionary<string, object> arguments = new Dictionary<string, object>() { { "x-message-ttl", 300000 } };
                //channel.QueueDeclare(routingKey, false, false, true, arguments);
                var properties = new BasicProperties();
                properties.DeliveryMode = DeliveryModes.Persistent;
                properties.Expiration = (ExpireSecond * 1000).ToString();
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublishAsync("", routingKey, false, properties, body);
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.Message, ex);
            }
        }
        public ConcurrentDictionary<string, TaskCompletionSource<RabbitMqTransaction>> waitTransactions = new ConcurrentDictionary<string, TaskCompletionSource<RabbitMqTransaction>>();
        public RabbitMqTransaction? ProduceWaitReply(string routingKey, RabbitMqTransaction trans)
        {
            try
            {
                trans.ReplyChannel = consumeSubQueue;
                var tcs = new TaskCompletionSource<RabbitMqTransaction>();
                var message = JsonConvert.SerializeObject(trans);
                var tid = trans.TransactionID;
                waitTransactions.TryAdd(tid, tcs);
                Produce(routingKey, message, trans.ExpireSecond);
                var completedTaskIndex = Task.WaitAny(new Task[] { tcs.Task }, trans.ExpireSecond * 1000);
                if (completedTaskIndex == 0)
                {
                    waitTransactions.TryRemove(tid, out _);
                    return tcs.Task.Result;
                }
                else
                {
                    waitTransactions.TryRemove(tid, out _);
                    dbgLog.Warn($"Tansaction {trans.TransactionName} timeout, {trans.ExpireSecond} s");
                    return null;
                }
            }
            catch (Exception ex)
            {
                dbgLog.Warn(ex.ToString());
                return null;
            }
        }

    }
}
