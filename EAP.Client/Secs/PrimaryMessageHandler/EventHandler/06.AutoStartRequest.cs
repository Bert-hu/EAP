using EAP.Client.Secs.Models;
using log4net;
using Secs4Net;
using static Secs4Net.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    class AutoStartRequest : IEventHandler
    {
        internal readonly ILog traLog = LogManager.GetLogger("Trace");
        internal readonly ISecsGem secsGem;
        public static List<string> bufferLots = new List<string>();
        public AutoStartRequest(SecsGem secsGem)
        {
            this.secsGem = secsGem;
        }


        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            traLog.Info($"5.SlotMapping Success.");
            traLog.Info($"6.AutoStart Start.");
            var s2f41AutoStart = new SecsMessage(2, 41)
            {
                SecsItem = L(
                    A("G100"),
                    L(
                        L(
                            A("SWITCH"),
                            U4(1)
                        )
                    )
                )
            };
            await secsGem.SendAsync(s2f41AutoStart);
            bufferLots.Add(CassetteIdReport.nextLot);
            CassetteIdReport.nextLot = string.Empty;
            traLog.Info($"6.AutoStart Success.");
        }
    }
}
