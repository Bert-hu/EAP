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
    internal class GetEPPD : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;
        public GetEPPD(RabbitMqService rabbitMq, ISecsGem secsGem) 
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
        }

        public  async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var recipename = string.Empty;
                SecsMessage s7f19 = new(7, 19, true)
                {
                };
                var rep = await secsGem.SendAsync(s7f19);
                List<string> EPPD = new List<string>();
                foreach (var item in rep.SecsItem.Items)
                {
                    EPPD.Add(item.GetString());
                }
                reptrans.Parameters.Add("Result", true);
                reptrans.Parameters.Add("EPPD", EPPD);

            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error {ex.Message}");
                dbgLog.Error(ex.Message, ex);
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }
    }
}
