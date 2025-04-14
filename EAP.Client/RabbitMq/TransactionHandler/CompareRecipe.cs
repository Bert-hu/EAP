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
                    SecsItem = L(U4(10101)//Recipe Para,
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

                    foreach (var s_groupPara in serverparas)
                    {
                        var m_groupPara = paras.FirstOrDefault(x => x.GroupName == s_groupPara.GroupName);
                        if (m_groupPara == null)
                        {
                            compareResult += $"{s_groupPara.GroupName} 未找到\n";
                            continue;
                        }
                        compareResult += CompareParameters(s_groupPara, m_groupPara);
                    }

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


        List<string> compareProperties = new List<string>()
        {
            "HeightUSL",
            "HeightLSL",
            "AreaUSL",
            "AreaLSL",
            "VolumeUSL",
            "VolumeLSL",
            "ShiftXUSL",
            "ShiftYUSL",
            "StencilHeight"
        };

        public string CompareParameters(SinictecSpiRecipeParameter param1, SinictecSpiRecipeParameter param2)
        {
            var differences = new List<string>();
            var properties = typeof(SinictecSpiRecipeParameter).GetProperties();

            foreach (var property in properties)
            {
                if (!compareProperties.Contains(property.Name)) continue;
                var value1 = property.GetValue(param1);
                var value2 = property.GetValue(param2);

                if (!Equals(value1, value2))
                {
                    differences.Add($"GroupName: {param1.GroupName}, {property.Name}: Server: {value1}, Machine: {value2}");
                }
            }

            return differences.Count > 0 ? string.Join("\n", differences) : string.Empty;
        }
    }
}
