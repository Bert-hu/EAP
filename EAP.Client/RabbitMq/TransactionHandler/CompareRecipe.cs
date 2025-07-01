using EAP.Client.Secs;
using log4net;
using Newtonsoft.Json;
using Secs4Net;
using System.Text;
using static Secs4Net.Item;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EAP.Client.RabbitMq
{
    internal class CompareRecipe : ITransactionHandler
    {
        internal readonly ILog dbgLog = LogManager.GetLogger("Debug");
        internal readonly ILog traLog = LogManager.GetLogger("Trace");


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
                byte[] recipebody = new byte[0];
                var recipeParameters = string.Empty;
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();
                if (trans.Parameters.TryGetValue("RecipeBody", out object _body)) recipebody = Convert.FromBase64String(_body.ToString());
                if (trans.Parameters.TryGetValue("RecipeParameters", out object _parameters)) recipeParameters = _parameters.ToString();

                var s1f3 = new SecsMessage(1, 3)
                {
                    SecsItem = L(
                                       U4(2002),
                                       U4(2015),
                                       U4(2016),
                                       U4(2017)
                                       )
                };
                var s1f4 = await secsGem.SendAsync(s1f3);
                var machineRecipeName = s1f4.SecsItem.Items[0].GetString();

                if (machineRecipeName != recipename)
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"请把recipe切换到{recipename}后再比较");
                    traLog.Error($"请把recipe切换到{recipename}后再比较");
                }
                else
                {
                    var serverPara = JsonConvert.DeserializeObject<Dictionary<string, string>>(recipeParameters);

                    var SetEnergy = s1f4.SecsItem.Items[1].GetString();
                    var EnergyUpperLimit = s1f4.SecsItem.Items[2].GetString();
                    var EnergyLowerLimit = s1f4.SecsItem.Items[3].GetString();

                    var machinePara = new Dictionary<string, string>();
                    machinePara.Add("RecipeName", machineRecipeName);
                    machinePara.Add("SetEnergy", SetEnergy);
                    machinePara.Add("EnergyUpperLimit", EnergyUpperLimit);
                    machinePara.Add("EnergyLowerLimit", EnergyLowerLimit);

                    var compareResult = CompareDictionaries(machinePara, serverPara);
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
