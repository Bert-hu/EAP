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


                SecsMessage s7f5 = new(7, 5, true)
                {
                    SecsItem = A(recipename)
                };
                var rep = await secsGem.SendAsync(s7f5);
                rep.Name = null;
                var reprecipename = rep.SecsItem[0].GetString();
                var data = rep.SecsItem[1].GetMemory<byte>().ToArray();
                if (data == recipebody)
                {
                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("Message", $"Compare body OK");
                }
                else
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"recipe内容不一致");
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
