using log4net;
using Newtonsoft.Json;
using Secs4Net;
using static Secs4Net.Item;
using static Secs4Net.Sml.SmlWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Globalization;
using Sunny.UI;
using EAP.Client.Forms;

namespace EAP.Client.RabbitMq.TransactionHandler
{
    class GetAllConfiguration : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;

        public GetAllConfiguration(RabbitMqService rabbitMq, ISecsGem secsGem)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
        }
        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var message = string.Empty;
                if (trans.Parameters.TryGetValue("Message", out object _message)) message = _message?.ToString();
                if (!string.IsNullOrEmpty(message))
                {
                    var s10f3 = new SecsMessage(10, 3)
                    {
                        SecsItem = L(
                  B(0),
                  A(message)
            )
                    };
                    await secsGem.SendAsync(s10f3);
                }


                string isheld = string.Empty;
                if (trans.Parameters.TryGetValue("IsHeld", out object _isheld)) isheld = _isheld?.ToString();
                MainForm.Instance.UpdateMachineLock(isheld.ToUpper() == "TRUE", message);

            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }

      
    }
}
