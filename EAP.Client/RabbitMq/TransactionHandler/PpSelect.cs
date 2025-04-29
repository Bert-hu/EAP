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

                var s1f3 = new SecsMessage(1, 3)
                {
                    SecsItem = L(
                        U4(239)
                        )
                };
                var s1f4 = await secsGem.SendAsync(s1f3);
                var plc_status = s1f4.SecsItem[0].FirstValue<ushort>();
                if (plc_status == 1)
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", "PLC Status is on Manual Mode");
                    return;
                }

                var reqindex = recipeName.Split('_')[0];

                var s2f41 = new SecsMessage(2, 41)
                {
                    SecsItem = L(
                        A("PPSELECT"),
                        L(
                            L(
                                  A("PPID"),
                                  A(reqindex)
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
