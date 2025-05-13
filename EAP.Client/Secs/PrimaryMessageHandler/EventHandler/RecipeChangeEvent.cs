using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Utils;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Secs4Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class RecipeChangeEvent : IEventHandler
    {
        private readonly ILog traLog = LogManager.GetLogger("Trace");

        private readonly RabbitMqService rabbitMqService;
        private readonly ISecsGem secsGem;
        private readonly IConfiguration configuration;

        public RecipeChangeEvent(RabbitMqService rabbitMqService, ISecsGem secsGem, IConfiguration configuration)
        {
            this.rabbitMqService = rabbitMqService;
            this.secsGem = secsGem;
            this.configuration = configuration;
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            try
            {
                //TODO: 更新混包配置



                var equipmentID = configuration.GetSection("Custom")["EquipmentId"];

                var packageName = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();
                var sealTemp = wrapper.PrimaryMessage.SecsItem[2][0][1][1].FirstValue<double>();
                var sealPress = wrapper.PrimaryMessage.SecsItem[2][0][1][2].FirstValue<uint>();
                var carrierTapeID = wrapper.PrimaryMessage.SecsItem[2][0][1][3].GetString();
                var coverTapeID = wrapper.PrimaryMessage.SecsItem[2][0][1][4].GetString();

                var data = new
                {
                    Equipment = equipmentID,
                    ConfigItems = new
                    {
                        PackageName = packageName,
                        SealTemp = sealTemp,
                        SealPress = sealPress,
                        CarrierTapeID = carrierTapeID,
                        CoverTapeID = coverTapeID
                    }
                };
                var faiUrl = configuration.GetSection("Custom")["FaiUrl"];

                var jsonBody = System.Text.Json.JsonSerializer.Serialize(data);

                var result = await HttpClientHelper.HttpPostRequestAsync<object>(faiUrl, data);
                traLog.Info($"FAI 触发结果：{faiUrl},{jsonBody},{result.ToString()}");

                MainForm.Instance.UpdatePackageName(packageName);
            }
            catch (Exception ex)
            {
                traLog.Error(ex.ToString());
            }
        }
    }
}
