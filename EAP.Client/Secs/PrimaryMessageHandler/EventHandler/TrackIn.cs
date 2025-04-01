using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using Microsoft.Extensions.Configuration;
using Secs4Net;
using static Secs4Net.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    class TrackIn : IEventHandler
    {
        private static readonly ILog traLog = log4net.LogManager.GetLogger("traLog");

        private readonly ISecsGem secsGem;
        private readonly IConfiguration configuration;

        public TrackIn(ISecsGem secsGem, IConfiguration configuration)
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

            string empno = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();
            string lotid = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();
            string ballid = wrapper.PrimaryMessage.SecsItem[2][0][1][2].GetString();
            string fluxid = wrapper.PrimaryMessage.SecsItem[2][0][1][3].GetString();

            string ballmsg = $"{equipmentId},{ballid},5,{empno},{line},,OK,,,MODEL_NAME={lotid}";
            //string fluxmsg = $"{equipmentId},{fluxid},5,{empno},{line},,OK,,,MODEL_NAME={lotid}";

            BaymaxService service = new BaymaxService();
            var ballTrans = service.GetBaymaxTrans(baymaxIp, baymaxPort, ballmsg);

            if (ballTrans.Result && ballTrans.BaymaxResponse.ToUpper().StartsWith("OK"))
            {
                var s2f41LotCheckOK = new SecsMessage(2, 41)
                {
                    SecsItem = L(
                            A("LOT_CHECK_CONFIRM"),
                            L(
                                L(
                                    A("RESULT"),
                                    A("OK")
                                    )
                                )
                        )
                };
                await secsGem.SendAsync(s2f41LotCheckOK);
            }
            else
            {
                var s2f41LotCheckNG = new SecsMessage(2, 41)
                {
                    SecsItem = L(
                              A("LOT_CHECK_CONFIRM"),
                              L(
                                  L(
                                      A("RESULT"),
                                      A("NG")
                                      )
                                  )
                          )
                };
                await secsGem.SendAsync(s2f41LotCheckNG);

                var errorMessage = $"SFIS Fail: {ballTrans.BaymaxResponse}" ;

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
