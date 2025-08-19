using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using log4net;
using Secs4Net;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class BarcodeScanned : IEventHandler
    {
        internal ILog traLog = LogManager.GetLogger("Trace");

        public static bool OnPpSelectStatus = false;
        public static string OnPpSelectStatusRecipeName = string.Empty;
        public static DateTime OnPpSelectStatusTime = DateTime.MinValue;

        private readonly ISecsGem secsGem;
        private RabbitMqService rabbitMqService;
        private readonly IServiceProvider serviceProvider;
        private readonly CommonLibrary commonLibrary;

        public BarcodeScanned(RabbitMqService rabbitMq, ISecsGem secsGem, IServiceProvider serviceProvider, CommonLibrary commonLibrary)
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
              
                var machinerecipename = wrapper.PrimaryMessage.SecsItem[2][0][1][2].GetString();
                var panelsn = wrapper.PrimaryMessage.SecsItem[2][0][1][3].GetString();

                MainForm.Instance.UpdateMachineRecipe(machinerecipename);

                string equipmentId = commonLibrary.CustomSettings["EquipmentId"];
                string sfisIp = commonLibrary.CustomSettings["SfisIp"];
                int sfisPort = Convert.ToInt32(commonLibrary.CustomSettings["SfisPort"]);
                string rmsApiUrl = commonLibrary.CustomSettings["RmsApiUrl"];
                //traLog.InfoFormat("BarcodeScanned: {0},{1}", panelsn, machinerecipename);
                traLog.Info($"BarcodeScanned: {panelsn},{machinerecipename}");
                // Send message to SFIS to get LOT INFO
                bool autoCheckRecipe = MainForm.Instance.isAutoCheckRecipe;

                BaymaxService baymax = new BaymaxService();
                var getModelProjextReq = $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_PROJECT_NAME_INFO=???";
                var trans = await baymax.GetBaymaxTrans(sfisIp, sfisPort, getModelProjextReq);
                if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                {

                    // Parse SFIS response to get model name and wafer IDs
                    Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                        .Where(keyValueArray => keyValueArray.Length == 2)
                        .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                    string modelName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[0];
                    string projectName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[1];

                    MainForm.Instance.UpdateAoiPanelAndModelname(panelsn, modelName);


                    if (autoCheckRecipe)
                    {
                        if (!machinerecipename.Contains(projectName))
                        {
                            var errMsg = $"Panelid: {panelsn},Recipe: {machinerecipename},ProjectName:{projectName},recipe 不匹配";
                            traLog.Warn(errMsg);
                            SendS10F3ToEquipment(secsGem, $"Panelid: {panelsn},Recipe: {machinerecipename},ProjectName:{projectName},recipe 不匹配");

                        }
                        else
                        {
                            var equipmentid = commonLibrary.CustomSettings["EquipmentId"];
                            var rabbitTrans = new RabbitMqTransaction()
                            {
                                TransactionName = "CompareRecipeBody",
                                EquipmentID = equipmentid,
                                Parameters = new Dictionary<string, object>() { { "EquipmentId", equipmentid }, { "RecipeName", machinerecipename }, },
                            };
                            var repTrans = rabbitMqService.ProduceWaitReply("Rms.Service", rabbitTrans);
                            if (repTrans != null)
                            {
                                var result = false;
                                var message = string.Empty;
                                if (repTrans.Parameters.TryGetValue("Result", out object _result)) result = (bool)_result;
                                if (repTrans.Parameters.TryGetValue("Message", out object _message)) message = _message.ToString();
                                if (!result)
                                {
                                    traLog.Warn("Compare recipe fail: " + message);
                                    SendS10F3ToEquipment(secsGem, "Compare recipe body fail: " + message);
                                }
                                else
                                {
                                    //比对没问题
                                }
                            }
                            else
                            {
                                traLog.Warn("Compare recipe fail: Timeout");
                            }
                        }
                    }

                }
                else
                {
                    traLog.Error($"BarcodeScanned: {trans.BaymaxResponse}");
                }

            }
            catch (Exception ex)
            {
                traLog.Error(ex);
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
