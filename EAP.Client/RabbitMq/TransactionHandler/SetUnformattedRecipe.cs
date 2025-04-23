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
    internal class SetUnformattedRecipe : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;
        internal readonly CommonLibrary commonLibrary;

        public SetUnformattedRecipe(RabbitMqService rabbitMq, ISecsGem secsGem, CommonLibrary commonLibrary)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
        }

        public  async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var Message = string.Empty;
                var recipename = string.Empty;
                string smlbody = string.Empty;
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();
                if (trans.Parameters.TryGetValue("RecipeBody", out object _body)) smlbody = _body.ToString();

                var message = commonLibrary.StringToSecsMessage(smlbody);
                var newmsg = new SecsMessage(7, 23)
                {
                    SecsItem = message.SecsItem
                };
                var s7f24 = await secsGem.SendAsync(newmsg);
                var s7f24ack = s7f24.SecsItem.FirstValue<byte>();
                switch (s7f24ack)
                {
                    case 1:
                        Message = "Denied";
                        break;
                    case 2:
                        Message = "Length error";
                        break;
                    case 3:
                        Message = "Reserved";
                        break;
                    case 4:
                        Message = "PPID not found";
                        break;
                    case 5:
                        Message = "Mode unsupported";
                        break;
                    default:
                        Message = "Other error";
                        break;
                }
                if (s7f24ack == 0)
                {
                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("Message", $"Success");
                }
                else
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"Equipment PP Send Fail, Reason: {Message}");
                }
            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error: {ex.Message}");
                dbgLog.Error(ex.Message, ex);
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }
    }
}
