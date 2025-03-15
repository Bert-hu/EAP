using log4net;
using Newtonsoft.Json;
using Secs4Net;
using EAP.Client.Secs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Secs4Net.Item;

namespace EAP.Client.RabbitMq
{
    internal class ReconnectMachine : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsConnection hsmsConnection;

        public ReconnectMachine(RabbitMqService rabbitMq, ISecsConnection hsmsConnection)
        {
            this.rabbitMq = rabbitMq;
            this.hsmsConnection = hsmsConnection;
        }

        public  Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                hsmsConnection.Reconnect();

                reptrans.Parameters.Add("Result", true);


            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error {ex.Message}");
                dbgLog.Error(ex.Message, ex);
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
            return Task.CompletedTask;
        }
    }
}
