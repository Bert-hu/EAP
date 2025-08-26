using EAP.Client.Models;
using EAP.Client.Secs;
using log4net;
using Newtonsoft.Json;
using Secs4Net;
using Secs4Net.Sml;
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

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var recipename = string.Empty;
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();

                //SecsMessage s7f5 = new(7, 5, true)
                //{
                //    SecsItem = A(recipename)
                //};
                //var rep = await secsGem.SendAsync(s7f5);
                //rep.Name = null;
                //var reprecipename = rep.SecsItem[0].GetString();
                //var data = rep.SecsItem[1].GetMemory<byte>().ToArray();
                //var strbody = Convert.ToBase64String(data);

                var s1f3 = new SecsMessage(1, 3, true)
                {
                    SecsItem = L(
                            U4(206),//packageName
                            U4(210)//ReelQuantity
                        )
                };
                var s1f4 = await secsGem.SendAsync(s1f3);
                var packageName = s1f4.SecsItem[0].GetString();
                var reelQuantity = s1f4.SecsItem[1].FirstValue<uint>();
                if (packageName != recipename)
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"请把程式切换到{recipename}后再上传");
                }
                else
                {
                    var recipeJson = JsonConvert.SerializeObject(new TapeReelPara
                    {
                        PackageName = packageName,
                        ReelQuantity = reelQuantity
                    });

                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("RecipeName", recipename);
                    reptrans.Parameters.Add("RecipeBody",  Convert.ToBase64String(new byte[] { }));
                    reptrans.Parameters.Add("RecipeParameters", recipeJson);
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
