using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using log4net;
using Microsoft.Extensions.Configuration;
using Secs4Net;
using static Secs4Net.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    class StripIDRead : IEventHandler
    {
        private static readonly ILog traLog = log4net.LogManager.GetLogger("traLog");

        private readonly ISecsGem secsGem;
        private readonly IConfiguration configuration;

        public StripIDRead(ISecsGem secsGem, IConfiguration configuration)
        {
            this.secsGem = secsGem;
            this.configuration = configuration;
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            string equipmentId = configuration.GetSection("Custom")["EquipmentId"];
            string line = configuration.GetSection("Custom")["Line"];

            string baymaxIp = configuration.GetSection("Custom")["BaymaxIp"] ?? "10.5.1.226";
            int baymaxPort = Convert.ToInt32(configuration.GetSection("Custom")["BaymaxPort"] ?? "21347");

            string panelSn = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();
            string empno = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();
            string model_name = wrapper.PrimaryMessage.SecsItem[2][0][1][2].GetString();

            string request = $"{equipmentId},{panelSn},2,{empno},{line},,OK,,,,,,,,,{model_name}";

            BaymaxService service = new BaymaxService();
            var baymaxTrans = service.GetBaymaxTrans(baymaxIp, baymaxPort, request);
            if (baymaxTrans.Result && baymaxTrans.BaymaxResponse.ToUpper().StartsWith("OK"))
            {
                var s2f41StripOK = new SecsMessage(2, 41)
                {
                    SecsItem = L(
                           A("STRIP_LOAD_CONFIRM"),
                           L(
                               L(
                                   A("RESULT"),
                                   A("OK")
                                   )
                               )
                       )
                };
                await secsGem.SendAsync(s2f41StripOK);
            }
            else
            {
                var s2f41StripNG = new SecsMessage(2, 41)
                {
                    SecsItem = L(
                    A("STRIP_LOAD_CONFIRM"),
                    L(
                        L(
                            A("RESULT"),
                            A("NG")
                            )
                        )
                )
                };
                await secsGem.SendAsync(s2f41StripNG);

                var errorMessage = $"SFIS Fail: {baymaxTrans.BaymaxResponse}";

                traLog.Error(errorMessage);
                SendS10F3ToEquipment(secsGem, errorMessage);
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
