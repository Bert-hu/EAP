using EAP.Client.Secs;
using log4net;
using Secs4Net;
using Secs4Net.Sml;
using static Secs4Net.Item;

namespace EAP.Client.RabbitMq
{
    internal class GetFormattedRecipe : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;

        public GetFormattedRecipe(RabbitMqService rabbitMq, ISecsGem secsGem) 
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
                SecsMessage s7f25 = new(7, 25, true)
                {
                    SecsItem = A(recipename)
                };
                var s7f26 = await secsGem.SendAsync(s7f25);
                s7f26.Name = null;
                var reprecipename = s7f26.SecsItem[0].GetString();

                if (s7f26.F == 26 && s7f26.SecsItem != null)
                {
                    var data = s7f26.ToSml();
                    //var strbody = Convert.ToBase64String(data);
                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("RecipeName", reprecipename);
                    reptrans.Parameters.Add("RecipeBody", data);
                    reptrans.Parameters.Add("RecipeParameterList", GetAllItems(s7f26.SecsItem));
                }
                else
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("RecipeName", reprecipename);
                    reptrans.Parameters.Add("Message", "Machine does not support formatted recipe!");
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

        private List<(int[], string format, string value)> GetAllItems(Item dataItem, int[] indices = null)
        {
            List<(int[], string format, string value)> itemList = new List<(int[], string format, string value)>();

            if (dataItem.Format == SecsFormat.List)
            {
                for (int i = 0; i < dataItem.Count; i++)
                {
                    int[] currentIndex = (indices ?? new int[] { 0 }).Append(i).ToArray();

                    switch (dataItem[i].Format)
                    {
                        case SecsFormat.List:
                            List<(int[], string format, string value)> subItemList = GetAllItems(dataItem[i], currentIndex);
                            itemList.AddRange(subItemList);
                            break;
                        case SecsFormat.ASCII:
                            itemList.Add((currentIndex, dataItem[i].Format.ToString(), Convert.ToString(dataItem[i].GetString())));
                            break;
                        case SecsFormat.Binary:
                            itemList.Add((currentIndex, dataItem[i].Format.ToString(), Convert.ToString(dataItem[i].FirstValue<byte>())));
                            break;
                        case SecsFormat.Boolean:
                            itemList.Add((currentIndex, dataItem[i].Format.ToString(), Convert.ToString(dataItem[i].FirstValue<bool>())));
                            break;
                        case SecsFormat.I1:
                        case SecsFormat.I2:
                        case SecsFormat.I4:
                        case SecsFormat.I8:
                        case SecsFormat.U1:
                        case SecsFormat.U2:
                        case SecsFormat.U4:
                        case SecsFormat.U8:
                            itemList.Add((currentIndex, dataItem[i].Format.ToString(), Convert.ToString(dataItem[i].FirstValue<byte>())));
                            break;
                        case SecsFormat.F4:
                            itemList.Add((currentIndex, dataItem[i].Format.ToString(), Convert.ToString(dataItem[i].FirstValue<float>())));
                            break;
                        case SecsFormat.F8:
                            itemList.Add((currentIndex, dataItem[i].Format.ToString(), Convert.ToString(dataItem[i].FirstValue<double>())));
                            break;
                        default:
                            itemList.Add((currentIndex, dataItem[i].Format.ToString(), "Unknown format"));
                            break;
                    }
                }
            }

            return itemList;
        }
    }
}
