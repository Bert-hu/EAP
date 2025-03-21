using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using log4net;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class WaferMarkingEnd : IEventHandler
    {
        private ILog traLog = LogManager.GetLogger("Trace");

        private readonly ISecsGem secsGem;
        private readonly RabbitMqService rabbitMqService;
        private readonly IServiceProvider serviceProvider;
        private readonly CommonLibrary commonLibrary;

        public WaferMarkingEnd(RabbitMqService rabbitMq, ISecsGem secsGem, IServiceProvider serviceProvider, CommonLibrary commonLibrary)
        {
            this.rabbitMqService = rabbitMq;
            this.secsGem = secsGem;
            this.serviceProvider = serviceProvider;
            this.commonLibrary = commonLibrary;
        }

        public Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            try
            {
                string waferId = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();
                var successCode = wrapper.PrimaryMessage.SecsItem[2][0][1][2].FirstValue<byte>();

                string equipmentId = commonLibrary.CustomSettings["EquipmentId"];
                string sfisIp = commonLibrary.CustomSettings["SfisIp"];
                int sfisPort = Convert.ToInt32(commonLibrary.CustomSettings["SfisPort"]);

                if (!string.IsNullOrEmpty(waferId))
                {
                    if (successCode == 3)
                    {
                        BaymaxService service = new BaymaxService();
                        var waferOut = $"{equipmentId}_WF_OUT,{waferId},2,M090616,JORDAN,,OK,";
                        var waferOutTrans = service.GetBaymaxTrans(sfisIp, sfisPort, waferOut);
                        if (!waferOutTrans.Result)
                        {
                            traLog.Error($"{waferId} Wafer In 自动过站失败，请检查");
                        }
                        else
                        {
                            traLog.Info($"{waferId} Wafer Out 自动过站成功");
                        }

                    }
                    else
                    {
                        traLog.Error($"Wafer Id WaferStatus Code 为 {successCode}，不过站Wafer Out");
                    }
                }
                else
                {
                    traLog.Error($"Wafer Id为空，不过站Wafer Out");
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"WaferMarkingEnd: {ex.ToString()}");
            }
            return Task.CompletedTask;
        }
    }
}
