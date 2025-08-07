using EAP.Client.Secs;
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
    internal class GetAgvLockState : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;
        internal readonly CommonLibrary commonLibrary;

        public GetAgvLockState(RabbitMqService rabbitMq, ISecsGem secsGem, CommonLibrary commonLibrary)
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
                var lockStateSvid = commonLibrary.GetGemSvid("LockState").ID;

                var s1f3 = new SecsMessage(1, 3)
                {
                    SecsItem = L(U4((uint)lockStateSvid))
                };
                var s1f4 = await secsGem.SendAsync(s1f3);
                if (s1f4.SecsItem[0].GetString().ToUpper() == "TRUE")
                {
                    reptrans.Parameters.Add("Result", true);
                }
                else
                {
                    reptrans.Parameters.Add("Result", false);
                }
            }
            catch (Secs4Net.SecsException)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"设备SECSGEM未连接");
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
