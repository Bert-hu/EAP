using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using EAP.Client.Utils;
using log4net;
using Microsoft.Extensions.Configuration;
using Secs4Net;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    class PpRequest : IEventHandler
    {
        internal readonly ILog traLog = LogManager.GetLogger("Trace");

        internal readonly ISecsGem secsGem;

        public PpRequest(SecsGem secsGem)
        {
            this.secsGem = secsGem;
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            traLog.Info($"2.Lotid Check Success.");
            traLog.Info($"3.PP-Select Start.");
            var s2f41Ppselect = new SecsMessage(2, 41)
            {
                SecsItem = L(
                    A("PP-SELECT"),
                    L(

                         L(
                            A("TASK-ID"),
                            A(CassetteIdReport.nextRecipe)
                        )
                    )

                )
            };
            await secsGem.SendAsync(s2f41Ppselect);
        }
    }
}
