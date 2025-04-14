using log4net;
using Newtonsoft.Json;
using Secs4Net;
using Secs4Net.Sml;
using EAP.Client.Secs;
using static Secs4Net.Item;
using EAP.Client.Models;

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

                var s1f3 = new SecsMessage(1, 3)
                {
                    SecsItem = L(
                       U4(10101)//Recipe Para,  
                   )
                };

                var s1f4 = secsGem.SendAsync(s1f3).Result;


                var currentRecipeName = s1f4.SecsItem.Items[0][0].GetString();
                if (currentRecipeName == recipename)
                {
                    List<SinictecSpiRecipeParameter> paras = new List<SinictecSpiRecipeParameter>();

                    for (int grp = 1; grp < s1f4.SecsItem.Items[0].Count; grp++)
                    {
                        var groupStr = s1f4.SecsItem.Items[0][grp].GetString();
                        var para = ParseParameter(groupStr);
                        paras.Add(para);
                    }

                    //SecsMessage s7f5 = new(7, 5, true)
                    //{
                    //    SecsItem = A(recipename)
                    //};
                    //var rep = secsGem.SendAsync(s7f5).Result;
                    //rep.Name = null;
                    //var reprecipename = rep.SecsItem[0].GetString();
                    var data = new byte[0];
                    var strbody = Convert.ToBase64String(data);
                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("RecipeName", recipename);
                    reptrans.Parameters.Add("RecipeBody", strbody);
                    reptrans.Parameters.Add("RecipeParameters", JsonConvert.SerializeObject(paras, Formatting.Indented));
                }
                else
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"GetUnformattedRecipe失败，当前程式是{currentRecipeName},请切换到{recipename}后重试");
                    dbgLog.Error($"GetUnformattedRecipe失败，当前程式是{currentRecipeName},请切换到{recipename}后重试");
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

        public static SinictecSpiRecipeParameter ParseParameter(string input)
        {
            var config = new SinictecSpiRecipeParameter();

            var pairs = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim();

                    switch (key)
                    {
                        case "HeightUSL":
                            config.HeightUSL = int.Parse(value.TrimEnd('(', '%', ')'));
                            break;
                        case "HeightLSL":
                            config.HeightLSL = int.Parse(value.TrimEnd('(', '%', ')'));
                            break;
                        case "AreaUSL":
                            config.AreaUSL = int.Parse(value.TrimEnd('(', '%', ')'));
                            break;
                        case "AreaLSL":
                            config.AreaLSL = int.Parse(value.TrimEnd('(', '%', ')'));
                            break;
                        case "VolumeUSL":
                            config.VolumeUSL = int.Parse(value.TrimEnd('(', '%', ')'));
                            break;
                        case "VolumeLSL":
                            config.VolumeLSL = int.Parse(value.TrimEnd('(', '%', ')'));
                            break;
                        case "ShiftXUSL":
                            config.ShiftXUSL = int.Parse(value.TrimEnd('(', '%', ')'));
                            break;
                        case "ShiftYUSL":
                            config.ShiftYUSL = int.Parse(value.TrimEnd('(', '%', ')'));
                            break;
                        case "ShapeUSL":
                            config.ShapeUSL = int.Parse(value.TrimEnd("(mm)".ToCharArray()));
                            break;
                        case "StencilHeight":
                            config.StencilHeight = double.Parse(value.TrimEnd("(mm)".ToCharArray()));
                            break;
                        case "BridgeHeight":
                            config.BridgeHeight = double.Parse(value.TrimEnd("(mm)".ToCharArray()));
                            break;
                        case "BridgeWidth":
                            config.BridgeWidth = double.Parse(value.TrimEnd("(mm)".ToCharArray()));
                            break;
                        case "BridgeDistance":
                            config.BridgeDistance = double.Parse(value.TrimEnd("(mm)".ToCharArray()));
                            break;
                        case "GroupName":
                            config.GroupName = value;
                            break;
                        case "BridgeType":
                            config.BridgeType = value;
                            break;
                        case "LightType":
                            config.LightType = value;
                            break;
                        case "VacuumChuck":
                            config.VacuumChuck = value;
                            break;
                        case "RU":
                            config.RU = int.Parse(value);
                            break;
                        case "RL":
                            config.RL = int.Parse(value);
                            break;
                        case "GU":
                            config.GU = int.Parse(value);
                            break;
                        case "GL":
                            config.GL = int.Parse(value);
                            break;
                        case "BU":
                            config.BU = int.Parse(value);
                            break;
                        case "BL":
                            config.BL = int.Parse(value);
                            break;
                    }
                }
            }

            return config;
        }
    }
}
