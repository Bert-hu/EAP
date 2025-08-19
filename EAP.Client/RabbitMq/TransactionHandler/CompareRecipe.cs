using EAP.Client.Model;
using EAP.Client.Models;
using EAP.Client.Secs;
using log4net;
using Newtonsoft.Json;
using Secs4Net;
using System.Text;
using static Secs4Net.Item;

namespace EAP.Client.RabbitMq
{
    internal class CompareRecipe : ITransactionHandler
    {
        internal readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;
        internal readonly ISecsConnection hsmsConnection;
        internal readonly CommonLibrary commonLibrary;

        public CompareRecipe(RabbitMqService rabbitMq, ISecsGem secsGem, ISecsConnection hsmsConnection, CommonLibrary commonLibrary)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
            this.hsmsConnection = hsmsConnection;
            this.commonLibrary = commonLibrary;


        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {


                var recipename = string.Empty;
                //byte[] recipebody = new byte[0];
                var recipeParameters = string.Empty;
                List<RMS_PARAMETER_SCOPE> scope = new List<RMS_PARAMETER_SCOPE>();

                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();
                //if (trans.Parameters.TryGetValue("RecipeBody", out object _body)) recipebody = Convert.FromBase64String(_body.ToString());
                if (trans.Parameters.TryGetValue("RecipeParameters", out object _parameters)) recipeParameters = _parameters.ToString();
                if (trans.Parameters.TryGetValue("RecipeParameterScope", out object _scope)) scope = JsonConvert.DeserializeObject<List<RMS_PARAMETER_SCOPE>>(JsonConvert.SerializeObject(_scope));

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
                    reptrans.Parameters.Add("Message", $"请把程式切换到{recipename}后再比较");
                }
                else
                {
                    var machinePara = new TapeReelPara
                    {
                        PackageName = packageName,
                        ReelQuantity = reelQuantity
                    };
                    var serverPara = JsonConvert.DeserializeObject<TapeReelPara>(recipeParameters);

                    var compareMessage = machinePara.CompareTo(serverPara, scope);
                    if (string.IsNullOrEmpty(compareMessage))
                    {
                        reptrans.Parameters.Add("Result", true);
                        reptrans.Parameters.Add("Message", "Compare OK");
                    }
                    else
                    {
                        reptrans.Parameters.Add("Result", false);
                        reptrans.Parameters.Add("Message", compareMessage);
                    }
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
