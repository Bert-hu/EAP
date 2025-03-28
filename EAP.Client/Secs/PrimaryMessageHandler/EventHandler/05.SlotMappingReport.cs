using EAP.Client.Secs.Models;
using log4net;
using Secs4Net;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    class SlotMappingReport : IEventHandler
    {
        internal readonly ILog traLog = LogManager.GetLogger("Trace");
        internal readonly ISecsGem secsGem;

        public SlotMappingReport(SecsGem secsGem)
        {
            this.secsGem = secsGem;
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            traLog.Info($"4.CassetteLoad Success.");
            traLog.Info($"5.SlotMapping Start.");
            uint portid = wrapper.PrimaryMessage.SecsItem[2][0][1][0].FirstValue<uint>();
            string slotstring = wrapper.PrimaryMessage.SecsItem[2][0][1][3].GetString();
            var slotInfos = slotstring.Select((c, index) => new { Character = c, Index = index });
            var s2f41SlotMapping = new SecsMessage(2, 41)
            {
                SecsItem = L(
                    A("SLOTMAPPING"),
                    L(
                        L(
                            A("PORT-ID"),
                            U4(portid)
                            ),
                        L(
                            from slotInfo in slotInfos
                            select
                            L(                                
                                A($"SLOT{slotInfo.Index}"),
                                A(slotInfo.Character.ToString())                                
                                )
                            )
                    ))
            };
            await secsGem.SendAsync(s2f41SlotMapping);
        }
    }
}
