using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using log4net;
using Microsoft.Extensions.Configuration;
using Secs4Net;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    class CassetteProcessEnd : IEventHandler
    {
        internal readonly ILog traLog = LogManager.GetLogger("Trace");
        internal readonly ISecsGem secsGem;
        internal readonly IConfiguration configuration;

        public CassetteProcessEnd(SecsGem secsGem, IConfiguration configuration)
        {
            this.secsGem = secsGem;
            this.configuration = configuration;
        }
        public Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            string lotid = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();
            if (!AutoStartRequest.bufferLots.Contains(lotid))
            {
                traLog.Info($"Manual Process End: {lotid}");
                return Task.CompletedTask;
            }
            AutoStartRequest.bufferLots.Remove(lotid);
            var eqid = configuration.GetSection("Custom")["EquipmentId"];
            var baymaxIp = configuration.GetSection("Custom")["SfisIp"] ?? "10.5.1.226";
            var baymaxPort = int.Parse(configuration.GetSection("Custom")["SfisPort"] ?? "21347");
            string step1Req = $"{eqid},{lotid},2,M068397,JORDAN,,OK,";

            BaymaxService baymaxService = new BaymaxService();
            var step1Trans = baymaxService.GetBaymaxTrans(baymaxIp, baymaxPort, step1Req);
            if (step1Trans.Result && step1Trans.BaymaxResponse.ToUpper().StartsWith("OK"))
            {
                traLog.Info($"{lotid} Lot End 自动过站成功");
            }
            else
            {
                traLog.Error($"{lotid} Lot End 自动过站失败: {step1Trans.BaymaxResponse}");
                SendS10F3ToEquipment(secsGem, $"{lotid} lot end fail: {step1Trans.BaymaxResponse}");
            }

            return Task.CompletedTask;
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
