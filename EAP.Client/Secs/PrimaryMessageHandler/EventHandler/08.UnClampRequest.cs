using EAP.Client.Secs.Models;
using log4net;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    class UnClampRequest : IEventHandler
    {
        internal readonly ILog traLog = LogManager.GetLogger("Trace");
        internal readonly ISecsGem secsGem;

        public UnClampRequest(SecsGem secsGem)
        {
            this.secsGem = secsGem;
        }
        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            traLog.Info($"7.CassetteUnload End.");
            traLog.Info($"8.UnClamp Start.");
            uint portid = wrapper.PrimaryMessage.SecsItem[2][0][1][0].FirstValue<uint>();
            var s2f41G003 = new SecsMessage(2, 41)
            {
                SecsItem = L(
                    A("G003"),
                    L(
                        L(
                            A("CLAMP"),
                            U4(0)
                        ),
                         L(
                            A("PORT-ID"),
                            U4(portid)
                        )
                    )     
                    )
                    };
            await secsGem.SendAsync(s2f41G003);
            traLog.Info($"8.UnClamp Success.");

        }
    }
}
