using log4net;
using Newtonsoft.Json;
using Secs4Net;
using static Secs4Net.Item;
using static Secs4Net.Sml.SmlWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Globalization;
using Sunny.UI;

namespace EAP.Client.RabbitMq.TransactionHandler
{
    class GetSvidValue : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;
        public GetSvidValue(RabbitMqService rabbitMq, ISecsGem secsGem)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
        }
        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                int[] vidList = { };
                if (trans.Parameters.TryGetValue("VidList", out object _vidList))
                {
                    vidList = JsonConvert.DeserializeObject<int[]>(JsonConvert.SerializeObject(_vidList));
                }

                var s1f3 = new SecsMessage(1, 3)
                {
                    SecsItem = L(
                              from vid in vidList
                              select U4((uint)vid)
                    )
                };
                var s1f4 = await secsGem.SendAsync(s1f3);

                Dictionary<string, object> para = new Dictionary<string, object>();
                for (int i = 0; i < s1f4.SecsItem.Count; i++)
                {
                    var type = s1f4.SecsItem[i].Format;
                    var value = GetItemValue(type, s1f4.SecsItem[i]);
                    para.Add(vidList[i].ToString(), new { Type = type, Value = value });
                }

            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }

        private string GetItemValue(SecsFormat format, Item item)
        {
            string value = string.Empty;
            switch (format)
            {
                case SecsFormat.ASCII:
                    value = item.GetString();
                    break;
                case SecsFormat.I1:
                    value = item.FirstValue<sbyte>().ToString();
                    break;
                case SecsFormat.I2:
                    value = item.FirstValue<short>().ToString();
                    break;
                case SecsFormat.I4:
                    value = item.FirstValue<int>().ToString();
                    break;
                case SecsFormat.I8:
                    value = item.FirstValue<long>().ToString();
                    break;
                case SecsFormat.U1:
                    value = item.FirstValue<byte>().ToString();
                    break;
                case SecsFormat.U2:
                    value = item.FirstValue<ushort>().ToString();
                    break;
                case SecsFormat.U4:
                    value = item.FirstValue<uint>().ToString();
                    break;
                case SecsFormat.U8:
                    value = item.FirstValue<ulong>().ToString();
                    break;
                case SecsFormat.F4:
                    value = item.FirstValue<float>().ToString();
                    break;
                case SecsFormat.F8:
                    value = item.FirstValue<double>().ToString();
                    break;
                case SecsFormat.Boolean:
                    value = item.FirstValue<bool>().ToString();
                    break;
                default:
                    value = string.Empty;
                    break;
            }
            return value;

        }
    }
}
