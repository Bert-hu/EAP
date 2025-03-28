using EAP.Client.Secs.Models;
using log4net;
using Secs4Net;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    class WaferCassetteLoadRequest : IEventHandler
    {
        internal readonly ILog traLog = LogManager.GetLogger("Trace");
        internal readonly ISecsGem secsGem;

        public WaferCassetteLoadRequest(SecsGem secsGem)
        {
            this.secsGem = secsGem;
        }
        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            traLog.Info($"3.PP-Select Success.");
            traLog.Info($"4.CassetteLoad Start.");
            uint portid = wrapper.PrimaryMessage.SecsItem[2][0][1][0].FirstValue<uint>();
            var s2f41G004 = new SecsMessage(2, 41)
            {
                SecsItem = L(
                    A("G004"),
                    L(
                        L(
                            A("LOAD"),
                            U4(1)
                        ),
                         L(
                            A("PORT-ID"),
                            U4(portid)
                        )
                    )

                )
            };
           await secsGem.SendAsync(s2f41G004);
        }
    }
}
