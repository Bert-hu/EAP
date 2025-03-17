using EAP.Client.Secs.Models;
using log4net;
using Secs4Net;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class InspectionResultXML : IEventHandler
    {
        private static readonly ILog traLog = log4net.LogManager.GetLogger("traLog");
        private readonly ISecsGem secsGem;
        private readonly CommonLibrary commonLibrary;

        public InspectionResultXML(ISecsGem secsGem, CommonLibrary commonLibrary)
        {
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
        }


        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            bool recmoteControl = commonLibrary.CustomSettings["RemoteControl"]?.ToUpper() == "TRUE";
            if (recmoteControl)
            {
                var linkedRecipeName = BarcodeScanned.OnPpSelectStatusRecipeName;
                if (BarcodeScanned.OnPpSelectStatus && BarcodeScanned.OnPpSelectStatusTime.AddMinutes(2) > DateTime.Now)//2分钟内的收到xml报告,发送停止到报告生成一般在一分钟左右
                {
                    traLog.Info($"Send PP-SELECT COMMAND '{linkedRecipeName}'");
                    var s2f41ppselect = new SecsMessage(2, 41)
                    {
                        SecsItem = L(
                            A("PP-SELECT"),
                            L(
                                L(
                                A("PPID"),
                                A(linkedRecipeName)
                                )
                                )
                            )
                    };
                    var s2f42ppselect = await secsGem.SendAsync(s2f41ppselect);
                    if (s2f42ppselect.SecsItem.Items[0].FirstValue<byte>() != 0)
                    {
                        SendS10F3ToEquipment(secsGem, $"PP-SELECT Fail, Code: {s2f42ppselect.SecsItem.Items[0].FirstValue<byte>()}");
                        return;
                    }
                }
                else//大于3分钟的xml报告直接关闭切换，以免出错
                {
                    traLog.Warn($"有超时的切换机种任务：{linkedRecipeName}");
                    BarcodeScanned.OnPpSelectStatus = false;
                    BarcodeScanned.OnPpSelectStatusTime = DateTime.MinValue;
                    BarcodeScanned.OnPpSelectStatusRecipeName = string.Empty;
                }
            }
        }

        internal void SendS10F3ToEquipment(ISecsGem secs, string message)
        {
            try
            {
                SecsMessage s10f3 = new(10, 3, false)
                {
                    SecsItem = L(
                        B(0x00),
                        A(message)
                        )
                };
                secs.SendAsync(s10f3);
            }
            catch (Exception ex)
            {
                traLog.Error(ex.ToString());
            }
        }
    }
}
