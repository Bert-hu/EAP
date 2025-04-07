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
    internal class PpSelect : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;

        public PpSelect(RabbitMqService rabbitMq, ISecsGem secsGem)
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
                        A("PP-SELECT"),
                        L(
                            L(
                                  A("PPID"),
                                  A(recipeName)
                                )
                            ))
                };
                await secsGem.SendAsync(s2f41);
                reptrans.Parameters.Add("Result", true);
                reptrans.Parameters.Add("Message", "Send PP-SELECT Success!");
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
