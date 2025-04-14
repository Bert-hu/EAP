using EAP.Client.Model;
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
                //var Message = string.Empty;
                var recipename = string.Empty;
                //byte[] recipebody = new byte[0];
                var recipeParameters = string.Empty;
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();
                //if (trans.Parameters.TryGetValue("RecipeBody", out object _body)) recipebody = Convert.FromBase64String(_body.ToString());
                if (trans.Parameters.TryGetValue("RecipeParameters", out object _parameters)) recipeParameters = _parameters.ToString();

                var recipePath = commonLibrary.CustomSettings["MachineRecipePath"];
                var filePath = Path.Combine(recipePath, recipename);
                if (!System.IO.File.Exists(filePath))
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"Machine Recipe '{recipename}' not found");
                }
                else
                {
                    var machineRecipeText = System.IO.File.ReadAllText(filePath);

                    var machineRecipePara = GetUnformattedRecipe.GetRecipePara(recipename, machineRecipeText);
                    var serverRecipePara = JsonConvert.DeserializeObject<RecipePara>(recipeParameters);

                    var compareResult = machineRecipePara.CompareTo(serverRecipePara);

                    if (string.IsNullOrEmpty(compareResult))
                    {
                        reptrans.Parameters.Add("Result", true);
                        reptrans.Parameters.Add("Message", "Compare OK");
                    }
                    else
                    {
                        reptrans.Parameters.Add("Result", false);
                        reptrans.Parameters.Add("Message", compareResult);
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
