using log4net;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Secs4Net.Item;

namespace EAP.Client.RabbitMq.TransactionHandler
{
    internal class AgvUnlock : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;

        public AgvUnlock(RabbitMqService rabbitMq, ISecsGem secsGem)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var recipeName = string.Empty;
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipeName = _rec?.ToString();
                var s2f41 = new SecsMessage(2, 41)
                {
                    SecsItem = L(
                        A("LOCKAGV_OFF"),
                        L(
                            L(

                                )
                            ))
                };
                var s2f42 = await secsGem.SendAsync(s2f41);
                if (s2f42.SecsItem.FirstValue<byte>() == 0)
                {
                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("Message", "Success!");
                }else
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"AGV Unlock Failed, Error Code: {s2f42.SecsItem.FirstValue<byte>()}");
                }
            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error {ex.Message}");
                dbgLog.Error(ex.ToString());
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }
    }
}
