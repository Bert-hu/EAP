using log4net;
using Newtonsoft.Json;
using Secs4Net;
using Secs4Net.Sml;
using EAP.Client.Secs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.RabbitMq
{
    internal class SendSecsMessage : ITransactionHandler
    {

        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;
        internal readonly CommonLibrary commonLibrary;
        public SendSecsMessage(RabbitMqService rabbitMq, ISecsGem secsGem, CommonLibrary commonLibrary)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var secsmsg = commonLibrary.StringToSecsMessage(trans.Parameters["Message"].ToString());
                var rep = await secsGem.SendAsync(secsmsg);
                rep.Name = null;
                if (secsmsg.ReplyExpected)
                {
                    reptrans.Parameters.Add("Message", rep.ToSml());
                }
                else
                {
                    reptrans.Parameters.Add("Message", "");
                }
                reptrans.Parameters.Add("Result", true);
            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                dbgLog.Error(ex.Message, ex);
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }
    }
}
