using log4net;
using Newtonsoft.Json;
using Secs4Net;
using Secs4Net.Sml;
using EAP.Client.Secs;
using static Secs4Net.Item;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EAP.Client.RabbitMq
{
    internal class GetUnformattedRecipe : ITransactionHandler
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");

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
                    reptrans.Parameters.Add("Message", $"请把recipe切换到{recipename}后再上传");
                    traLog.Error($"请把recipe切换到{recipename}后再上传");
                }
                else
                {
                    var SetEnergy = s1f4.SecsItem.Items[1].GetString();
                    var EnergyUpperLimit = s1f4.SecsItem.Items[2].GetString();
                    var EnergyLowerLimit = s1f4.SecsItem.Items[3].GetString();



                    SecsMessage s7f5 = new(7, 5, true)
                    {
                        SecsItem = A(recipename)
                    };
                    var rep = await secsGem.SendAsync(s7f5);
                    rep.Name = null;
                    var reprecipename = rep.SecsItem[0].GetString();
                    var data = rep.SecsItem[1].GetMemory<byte>().ToArray();
                    var strbody = Convert.ToBase64String(data);
                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("RecipeName", reprecipename);
                    reptrans.Parameters.Add("RecipeBody", strbody);

                    var para = new Dictionary<string, string>();
                    para.Add("RecipeName", machineRecipeName);
                    para.Add("SetEnergy", SetEnergy);
                    para.Add("EnergyUpperLimit", EnergyUpperLimit);
                    para.Add("EnergyLowerLimit", EnergyLowerLimit);
                    reptrans.Parameters.Add("RecipeParameters", JsonConvert.SerializeObject(para, Formatting.Indented));
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
