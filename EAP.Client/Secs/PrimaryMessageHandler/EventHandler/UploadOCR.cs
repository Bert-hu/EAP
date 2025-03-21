using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using log4net;
using Secs4Net;
using static Secs4Net.Item;
namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class UploadOCR : IEventHandler
    {
        private ILog traLog = LogManager.GetLogger("Trace");

        private readonly ISecsGem secsGem;
        private readonly RabbitMqService rabbitMqService;
        private readonly IServiceProvider serviceProvider;
        private readonly CommonLibrary commonLibrary;

        public UploadOCR(RabbitMqService rabbitMq, ISecsGem secsGem, IServiceProvider serviceProvider, CommonLibrary commonLibrary)
        {
            this.rabbitMqService = rabbitMq;
            this.secsGem = secsGem;
            this.serviceProvider = serviceProvider;
            this.commonLibrary = commonLibrary;
        }
        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            try
            {
                // Get wafer ID
                string waferId = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();

                // Get equipment ID, SFIS IP, SFIS port and RMS API URL from configuration
                string equipmentId = commonLibrary.CustomSettings["EquipmentId"];
                string sfisIp = commonLibrary.CustomSettings["SfisIp"];
                int sfisPort = Convert.ToInt32(commonLibrary.CustomSettings["SfisPort"]);
                string rmsApiUrl = commonLibrary.CustomSettings["RmsApiUrl"];

                // Send message to SFIS to get WAFER ID

                //(bool success, string sfisResponse, string errorMessage) = await SendMessageToSfisAsync(sfisIp, sfisPort, $"{equipmentId},{waferId},1,M090616,JORDAN,,OK,");

                BaymaxService service = new BaymaxService();
                var trans = service.GetBaymaxTrans(sfisIp, sfisPort, $"{equipmentId},{waferId},1,M090616,JORDAN,,OK,");


                if (trans.Result)
                {
                    // Send message to equipment based on SFIS response
                    SecsMessage s2f41;
                    if (trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                    {
                        s2f41 = new SecsMessage(2, 41)
                        {
                            SecsItem = L(
                                A("WAFER-CONTINUE"),
                                L())
                        };
                        await secsGem.SendAsync(s2f41);
                        //TODO: Wafer In
                        var waferIn = $"{equipmentId}_WF_IN,{waferId},2,M090616,JORDAN,,OK,";
                        var waferInTrans = service.GetBaymaxTrans(sfisIp, sfisPort, waferIn);
                        if (!waferInTrans.Result)
                        {
                            traLog.Error($"{waferId} Wafer In 自动过站失败，请检查");
                        }
                        else
                        {
                            traLog.Info($"{waferId} Wafer In 自动过站成功");
                            MainForm.Instance.UpdateWaferId(waferId);
                        }
                    }
                    else
                    {
                        s2f41 = new SecsMessage(2, 41)
                        {
                            SecsItem = L(
                                A("WAFER-REJECT"),
                                L())
                        };
                        await secsGem.SendAsync(s2f41);
                    }
            
                }
                else
                {
                    // Send error message to equipment
                    SendS10F3ToEquipment(secsGem, $"EAP FAIL: {trans.BaymaxResponse}");
                }
            }
            catch (Exception ex)
            {
                // Log exception and send error message to equipment
                traLog.Error($"An exception occurred: {ex}");
                SendS10F3ToEquipment(secsGem, $"An exception occurred: {ex.Message}");
            }
        }

        internal void SendS10F3ToEquipment(ISecsGem secs, string message)
        {
            try
            {
                SecsMessage s10f3 = new(10, 3, true)
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
