using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using log4net;
using Microsoft.Extensions.Configuration;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    class CassetteIdReport : IEventHandler
    {
        internal readonly ILog traLog = LogManager.GetLogger("Trace");

        internal readonly ISecsGem secsGem;
        internal readonly IConfiguration configuration;
        public static string nextLot = string.Empty;
        public CassetteIdReport(SecsGem secsGem, IConfiguration configuration)
        {
            this.secsGem = secsGem;
            this.configuration = configuration;
        }

        public Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            traLog.Info($"1.Clamp Success.");
            traLog.Info($"2.Lotid Check Start.");
            uint portid = wrapper.PrimaryMessage.SecsItem[2][0][1][0].FirstValue<uint>();
            string nextLot = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();

            var eqid = configuration.GetSection("Custom")["EquipmentId"];
            var baymaxIp = configuration.GetSection("Custom")["SfisIp"];
            var baymaxPort = int.Parse(configuration.GetSection("Custom")["SfisPort"] ?? "21347");
            string sfis_step1_req = $"{eqid},{nextLot},1,M068397,JORDAN,,OK,";
            BaymaxService service = new BaymaxService();
            var trans = service.GetBaymaxTrans(baymaxIp, baymaxPort, sfis_step1_req);
            if (trans.Result)
            {

            }
            else
            { 
            }
        }
    }
}
