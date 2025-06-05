using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;

using System.Collections.Concurrent;
using System.Text;

namespace EAP.Client.RabbitMq
{
    public class RabbitMqService
    {
        private readonly IConfiguration configuration;

        ConnectionFactory factory;
        public IConnection connection;
        public IChannel channel;
        internal static ILog dbgLog = LogManager.GetLogger("Debug");

        public RabbitMqService
            (IConfiguration configuration)
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

            connection = factory.CreateConnectionAsync().Result;
            channel = connection.CreateChannelAsync().Result;


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
                dbgLog.Info($"RabbitMqService Send message to {routingKey}: " + message);
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
                trans.ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"];
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
