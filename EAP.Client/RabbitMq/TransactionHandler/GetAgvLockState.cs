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
        public static Dictionary<string, string> StatusDict = new Dictionary<string, string>
        {
            { "0", "INIT" },
            { "1", "IDLE" },
            { "2", "SETUP" },
            { "3", "READY" },
            { "4", "EXECUTING" },
            { "5", "PAUSE" },
            { "6", "ALARM_PAUSE" },
            { "7", "IDLE_WITH_ALARMS" },
            { "8", "EXIT" }
        };
        public GetAgvLockState(RabbitMqService rabbitMq, ISecsGem secsGem, CommonLibrary commonLibrary)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            var processState = "UNKNOWN";
            try
            {
                var lockStateSvid = commonLibrary.GetGemSvid("LockState").ID;
                var processStateSvid = commonLibrary.GetGemSvid("ProcessState").ID;

                var s1f3 = new SecsMessage(1, 3)
                {
                    SecsItem = L(U4((uint)lockStateSvid), U4((uint)processStateSvid))
                };
                var s1f4 = await secsGem.SendAsync(s1f3);

                var processStateCode = s1f4.SecsItem[1].GetString();
                StatusDict.TryGetValue(processStateCode, out processState);
                if (string.IsNullOrEmpty(processState)) processState = "UNKNOWN";
                if (s1f4.SecsItem[0].GetString().ToUpper() == "TRUE")
                {
                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("ProcessState", processState);
                }
                else
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"设备未锁定");
                    reptrans.Parameters.Add("ProcessState", processState);
                }
            }
            catch (Secs4Net.SecsException)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"设备SECSGEM未连接");
                reptrans.Parameters.Add("ProcessState", processState);
            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error {ex.Message}");
                reptrans.Parameters.Add("ProcessState", processState);
                dbgLog.Error(ex.ToString());
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }
    }
}
