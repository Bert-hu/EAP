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
    internal class DeleteAllRecipes : ITransactionHandler
    {
        public RabbitMqTransaction reptrans;

        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;

        public DeleteAllRecipes(RabbitMqService rabbitMq, ISecsGem secsGem) 
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
        }

        public  async Task HandleTransaction(RabbitMqTransaction trans)
        {
            reptrans = trans?.GetReplyTransaction();
            try
            {
                var recipename = string.Empty;
                SecsMessage s7f17 = new(7, 17, true)
                {
                    SecsItem = L()
                };
                var s7f18 = await secsGem.SendAsync(s7f17);

                var s7f18ack = s7f18.SecsItem.FirstValue<byte>();
                if (s7f18ack == 0)
                {
                    reptrans.Parameters.Add("Result", true);
                }
                else
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", "Delete recipe fail");
                }
            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error {ex.Message}");
                dbgLog.Error(ex.Message, ex);
            }
            if (trans?.NeedReply ?? false)
            {
                rabbitMq.Produce(trans.ReplyChannel, reptrans);
            }
        }
    }
}
