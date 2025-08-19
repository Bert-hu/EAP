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
        public SetUnformattedRecipe(RabbitMqService rabbitMq, ISecsGem secsGem)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem; 
        }

        public  async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                //var Message = string.Empty;
                //var recipename = string.Empty;
                //byte[] recipebody = new byte[0];
                //if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();
                //if (trans.Parameters.TryGetValue("RecipeBody", out object _body)) recipebody = Convert.FromBase64String(_body.ToString());
                //SecsMessage s7f1 = new(7, 1, true)
                //{
                //    SecsItem =
                //    L(
                //        A(recipename),
                //        U4((uint)recipebody.Length)
                //    )
                //};
                //var s7f2 = await secsGem.SendAsync(s7f1);
                //var s7f2ack = s7f2.SecsItem.FirstValue<byte>();
                //switch (s7f2ack)
                //{
                //    case 1:
                //        Message = "Load already";
                //        break;
                //    case 2:
                //        Message = "No space";
                //        break;
                //    case 3:
                //        Message = "Invalid PPID";
                //        break;
                //    case 4:
                //        Message = "Busy, try again";
                //        break;
                //    case 5:
                //        Message = "Denied";
                //        break;
                //    default:
                //        Message = "Other error";
                //        break;
                //}
                //if (s7f2ack == 0)
                //{

                //    SecsMessage s7f3 = new(7, 3, true)
                //    {
                //        SecsItem =
                //        L(
                //            A(recipename),
                //            B(recipebody)
                //        )
                //    };
                //    var s7f4 = await secsGem.SendAsync(s7f3);
                //    var s7f4ack = s7f4.SecsItem.FirstValue<byte>();
                //    switch (s7f4ack)
                //    {
                //        case 1:
                //            Message = "Denied";
                //            break;
                //        case 2:
                //            Message = "Length error";
                //            break;
                //        case 3:
                //            Message = "Reserved";
                //            break;
                //        case 4:
                //            Message = "PPID not found";
                //            break;
                //        case 5:
                //            Message = "Mode unsupported";
                //            break;
                //        default:
                //            Message = "Other error";
                //            break;
                //    }
                //    if (s7f4ack == 0)
                //    {
                //        reptrans.Parameters.Add("Result", true);
                //        reptrans.Parameters.Add("Message", $"Success");
                //    }
                //    else//PPI FAIL
                //    {
                //        reptrans.Parameters.Add("Result", false);
                //        reptrans.Parameters.Add("Message", $"Equipment PP Send Fail, Reason: {Message}");
                //    }
                //}
                //else
                //{
                //    reptrans.Parameters.Add("Result", false);
                //    reptrans.Parameters.Add("Message", $"Equipment Inquire Fail, Reason: {Message}");
                //}

                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"This type of machine can not download recipe.");
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
