using log4net;
using Newtonsoft.Json;
using Secs4Net;
using Secs4Net.Sml;
using EAP.Client.Secs;
using static Secs4Net.Item;

namespace EAP.Client.RabbitMq
{
    internal class GetUnformattedRecipe : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;

        public GetUnformattedRecipe(RabbitMqService rabbitMq, ISecsGem secsGem) 
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
        }

        public  async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var recipename = string.Empty;
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();

                var reqindex = recipename.Split('_')[0];
                SecsMessage s7f25 = new(7, 25, true)
                {
                    SecsItem = A(reqindex)
                };
                var s7f26 = await secsGem.SendAsync(s7f25);
                s7f26.Name = null;
                var reprecipename = s7f26.SecsItem[0].GetString();

                if (s7f26.F == 26 && s7f26.SecsItem != null)
                {
                    byte[] bytedata = new byte[];
                    var data = s7f26.SecsItem.EncodeTo();
                    //var strbody = Convert.ToBase64String(data);
                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("RecipeName", reprecipename);
                    reptrans.Parameters.Add("RecipeBody", data);
                    reptrans.Parameters.Add("RecipeParameters", data);
                }
                else
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("RecipeName", reprecipename);
                    reptrans.Parameters.Add("Message", "Machine does not support formatted recipe!");
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
