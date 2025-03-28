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

                Dictionary<string, string> serverParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(recipeParameters);

                SecsMessage s7f25 = new(7, 25, true)
                {
                    SecsItem = A(recipename)
                };
                var s7f26 = secsGem.SendAsync(s7f25).Result;
                Dictionary<string, string> machineParameter = new Dictionary<string, string>();
                foreach (Item item in s7f26.SecsItem.Items[3].Items)
                {
                    var ccCode = item.Items[0].FirstValue<ushort>();
                    if (GetUnformattedRecipe.ccCodeNameDic.ContainsKey(ccCode))
                    {
                        var ccName = GetUnformattedRecipe.ccCodeNameDic[ccCode];
                        var ccType = GetUnformattedRecipe.ccCodeTypeDic[ccCode];
                        string ccValue = string.Empty;
                        if (ccType == typeof(uint))
                        {
                            ccValue = item.Items[1].FirstValue<uint>().ToString();
                        }
                        else if (ccType == typeof(int))
                        {
                            ccValue = item.Items[1].FirstValue<int>().ToString();
                        }
                        else if (ccType == typeof(string))
                        {
                            ccValue = item.Items[1].GetString();
                        }
                        else if (ccType == typeof(ushort))
                        {
                            ccValue = item.Items[1].FirstValue<ushort>().ToString();
                        }
                        machineParameter.Add(ccName, ccValue);
                    }


                    var compareResult = CompareDictionaries(machineParameter, serverParameters);
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

        public string CompareDictionaries(Dictionary<string, string> machine, Dictionary<string, string> server)
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
