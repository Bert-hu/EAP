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
                byte[] recipebody = new byte[0];
                var recipeParameters = string.Empty;
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();
                if (trans.Parameters.TryGetValue("RecipeBody", out object _body)) recipebody = Convert.FromBase64String(_body.ToString());
                if (trans.Parameters.TryGetValue("RecipeParameters", out object _parameters)) recipeParameters = _parameters.ToString();

                var s1f3 = new SecsMessage(1, 3)
                {
                    SecsItem = L(
      U4(10101)//Recipe Para,  
  )
                };

                var s1f4 = secsGem.SendAsync(s1f3).Result;

                var currentRecipeName = s1f4.SecsItem.Items[0][0].GetString();

                if (currentRecipeName != recipename)
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"Compare Recipe失败，当前程式是{currentRecipeName},请切换到{recipename}后重试");
                    dbgLog.Error($"Compare Recipe失败，当前程式是{currentRecipeName},请切换到{recipename}后重试");
                }
                else
                {
                    List<SinictecSpiRecipeParameter> paras = new List<SinictecSpiRecipeParameter>();

                    for (int grp = 1; grp < s1f4.SecsItem.Items[0].Count; grp++)
                    {
                        var groupStr = s1f4.SecsItem.Items[0][grp].GetString();
                        var para = GetUnformattedRecipe.ParseParameter(groupStr);
                        paras.Add(para);
                    }

                    //var machineParaStr = JsonConvert.SerializeObject(paras, Formatting.Indented);
                    var compareResult = string.Empty;
                    var serverparas = JsonConvert.DeserializeObject<List<SinictecSpiRecipeParameter>>(recipeParameters);


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


        public string CompareDictionaries(Dictionary<string, object> machine, Dictionary<string, object> server)
        {
            StringBuilder result = new StringBuilder();
            foreach (var kvPair in machine)
            {
                if (server.ContainsKey(kvPair.Key))
                {
                    if (server[kvPair.Key].ToString() != kvPair.Value.ToString())
                    {
                        result.AppendLine($"{kvPair.Key} Mahine:{kvPair.Value} vs Server:{server[kvPair.Key]}");

                    }
                }
            }
            return result.ToString();
        }
    }
}
