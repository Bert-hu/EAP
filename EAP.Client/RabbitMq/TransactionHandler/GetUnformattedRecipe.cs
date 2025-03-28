using log4net;
using Newtonsoft.Json;
using Secs4Net;
using Secs4Net.Sml;
using EAP.Client.Secs;
using static Secs4Net.Item;

namespace EAP.Client.RabbitMq
{
    internal class GetUnformattedRecipe : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;

        public static Dictionary<ushort, Type> ccCodeTypeDic = new Dictionary<ushort, Type>
 {
     { 0, typeof(ushort) },
     { 1, typeof(uint) },
     { 6, typeof(uint) },
     { 7, typeof(uint) },
     { 8, typeof(uint) },
     { 22, typeof(uint) },
     { 23, typeof(uint) },
     { 27, typeof(uint) },
     { 100, typeof(uint) },
     { 140, typeof(uint) },
     { 180, typeof(uint) },
     { 184, typeof(uint) },
     { 200, typeof(uint) },
     { 201, typeof(uint) },
     { 203, typeof(uint) },
     { 210, typeof(uint) },
     { 220, typeof(uint) },
     { 221, typeof(int) },
     { 222, typeof(uint) },
     { 223, typeof(uint) },
     { 224, typeof(uint) },
     { 225, typeof(uint) },
     { 226, typeof(uint) },
     { 227, typeof(uint) },
     { 228, typeof(uint) },
     { 229, typeof(uint) },
     { 230, typeof(uint) },
     { 231, typeof(uint) },
     { 232, typeof(uint) },
     { 233, typeof(uint) },
     { 234, typeof(uint) },
     { 235, typeof(uint) },
     { 236, typeof(uint) },
     { 237, typeof(uint) },
     { 238, typeof(int) },
     { 239, typeof(int) },
     { 240, typeof(int) },
     { 241, typeof(int) },
     { 242, typeof(uint) },
     { 243, typeof(uint) },
     { 244, typeof(uint) },
     { 260, typeof(uint) },
     { 270, typeof(uint) },
     { 430, typeof(uint) },
     { 436, typeof(uint) },
     { 450, typeof(string) },
     { 453, typeof(uint) },
     { 488, typeof(uint) },
     { 489, typeof(uint) },
     { 520, typeof(uint) },
     { 575, typeof(uint) },
     { 580, typeof(uint) },
     { 581, typeof(uint) },
     { 588, typeof(uint) },
     { 589, typeof(uint) },
     { 602, typeof(uint) },
     { 650, typeof(uint) },
     { 651, typeof(uint) },
     { 652, typeof(uint) },
     { 655, typeof(uint) },
     { 656, typeof(uint) },
     { 657, typeof(uint) },
     { 658, typeof(uint) },
     { 670, typeof(uint) },
     { 724, typeof(int) },
     { 745, typeof(uint) },
     { 760, typeof(uint) },
     { 762, typeof(int) },
     { 801, typeof(uint) },
     { 802, typeof(uint) },
     { 860, typeof(int) },
     { 9999, typeof(uint) }
 };

        public static Dictionary<ushort, string> ccCodeNameDic = new Dictionary<ushort, string>
 {
     { 1, "WaferSize" },
     { 436, "OFVnotchattachdir" },
     { 575, "Wafertableheatcond" },
     { 602, "PressrollerXattachspeed" },
     { 655, "Pressrollerpressure" },
     { 656, "PressrollerPresrAset" },
     { 657, "PressrollerPresrBset" },
     { 658, "PressrollerPresrCset" },
     { 670, "Pressrollerheatcond" },
     { 745, "Attachmenttapecond" }
 };

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

                SecsMessage s7f5 = new(7, 5, true)
                {
                    SecsItem = A(recipename)
                };
                var rep = await secsGem.SendAsync(s7f5);
                rep.Name = null;
                var reprecipename = rep.SecsItem[0].GetString();
                var data = rep.SecsItem[1].GetMemory<byte>().ToArray();

                SecsMessage s7f25 = new(7, 25, true)
                {
                    SecsItem = A(recipename)
                };
                var s7f26 = secsGem.SendAsync(s7f25).Result;
                Dictionary<string, string> ccParameter = new Dictionary<string, string>();
                foreach (Item item in s7f26.SecsItem.Items[3].Items)
                {
                    var ccCode = item.Items[0].FirstValue<ushort>();
                    if (ccCodeNameDic.ContainsKey(ccCode))
                    {
                        var ccName = ccCodeNameDic[ccCode];
                        var ccType = ccCodeTypeDic[ccCode];
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
                        ccParameter.Add(ccName, ccValue);
                    }
                }
                dbgLog.Info(JsonConvert.SerializeObject(ccParameter, Formatting.Indented));

                var strbody = Convert.ToBase64String(data);
                reptrans.Parameters.Add("Result", true);
                reptrans.Parameters.Add("RecipeName", reprecipename);
                reptrans.Parameters.Add("RecipeBody", strbody);
                reptrans.Parameters.Add("RecipeParameters", JsonConvert.SerializeObject(ccParameter, Formatting.Indented));

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
