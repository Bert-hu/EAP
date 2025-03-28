using EAP.Client.Secs.Models;
using log4net;
using Secs4Net;
using System;
using static Secs4Net.Item;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    class WaferCassetteUnloadRequest : IEventHandler
    {
        internal readonly ILog traLog = LogManager.GetLogger("Trace");
        internal readonly ISecsGem secsGem;

        public WaferCassetteUnloadRequest(SecsGem secsGem)
        {
            this.secsGem = secsGem;
        }
        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            traLog.Info("$7.CassetteUnload Start.");
            uint portid = wrapper.PrimaryMessage.SecsItem[2][0][1][0].FirstValue<uint>();
            var s2f41G004 = new SecsMessage(2, 41)
            {
                SecsItem = L(
                    A("G004"),
                    L(
                        L(
                            A("LOAD"),
                            U4(0)
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
