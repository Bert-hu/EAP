using log4net;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Secs4Net.Item;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EAP.Client.RabbitMq.TransactionHandler
{
    internal class StartCommand : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly ILog traLog = LogManager.GetLogger("Trace");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;

        public StartCommand(RabbitMqService rabbitMq, ISecsGem secsGem)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var s2f41 = new SecsMessage(2, 41)
                {
                    SecsItem = L(A("START"), L())
                };
                var s2f42 = await secsGem.SendAsync(s2f41);
                if (s2f42.SecsItem[0].FirstValue<byte>() == 0)
                {
                    traLog.Info($"远程Start命令执行成功!");
                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("Message", "Success!");
                }
                else
                {
                    traLog.Info($"远程Start命令执行失败!");
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"AGV Lock Failed, Error Code: {s2f42.SecsItem.FirstValue<byte>()}");
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
