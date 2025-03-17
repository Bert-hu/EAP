using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using Secs4Net;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class BarcodeScanned : IEventHandler
    {
        private static readonly log4net.ILog dbgLog = log4net.LogManager.GetLogger("dbgLog");
        private static readonly log4net.ILog traLog = log4net.LogManager.GetLogger("traLog");

        private readonly ISecsGem secsGem;
        private readonly RabbitMqService rabbitMqService;
        private readonly CommonLibrary commonLibrary;

        public static bool OnPpSelectStatus = false;
        public static string OnPpSelectStatusRecipeName = string.Empty;
        public static DateTime OnPpSelectStatusTime = DateTime.MinValue;

        public BarcodeScanned(RabbitMqService rabbitMq, ISecsGem secsGem, IServiceProvider serviceProvider, CommonLibrary commonLibrary)
        {
            this.rabbitMqService = rabbitMq;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
        }



        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            try
            {
                var panelsn = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();
                var machinerecipename = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();

                MainForm.Instance.UpdateMachineRecipe(machinerecipename);

                string equipmentId = commonLibrary.CustomSettings["EquipmentId"];
                string sfisIp = commonLibrary.CustomSettings["SfisIp"];
                int sfisPort = Convert.ToInt32(commonLibrary.CustomSettings["SfisPort"]);
                string rmsApiUrl = commonLibrary.CustomSettings["RmsApiUrl"];
                //traLog.InfoFormat("BarcodeScanned: {0},{1}", panelsn, machinerecipename);
                traLog.Info($"BarcodeScanned: {panelsn},{machinerecipename}");
                // Send message to SFIS to get LOT INFO
                BaymaxService baymax = new BaymaxService();
                 var baymaxTrans = baymax.GetBaymaxTrans(sfisIp, sfisPort, $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_INFO=???");

                //(bool success, string sfisResponse, string errorMessage) = SendMessageToSfis(sfisIp, sfisPort, $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_INFO=???");

                bool autoCheckRecipe = MainForm.Instance.isAutoCheckRecipe;
                if (baymaxTrans.Result)
                {
                    if (baymaxTrans.BaymaxResponse.ToUpper().StartsWith("OK"))
                    {
                        // Parse SFIS response to get model name and wafer IDs
                        Dictionary<string, string> sfisParameters = baymaxTrans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                            .Where(keyValueArray => keyValueArray.Length == 2)
                            .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                        string modelName = sfisParameters["SN_MODEL_NAME_INFO"];
                        MainForm.Instance.UpdateAoiPanelAndModelname(panelsn, modelName);

                        //(string linkedRecipeName, string recipeErrorMessage) = CheckRecipeGroup(rmsApiUrl, equipmentId, modelName);
                        string linkedRecipeName = GetAoiRelatedRecipe(secsGem, modelName);

                        if (linkedRecipeName == null)
                        {
                            if (autoCheckRecipe)
                            {
                                MainForm.Instance.ConfirmMessageBox("未找到关联的recipe");
                            }
                        }
                        else if (linkedRecipeName != machinerecipename + ".recipe")
                        {
                            traLog.Warn($"Panelid: {panelsn},ModelName:{modelName},recipe mismatch: {linkedRecipeName} != {machinerecipename + ".recipe"}");
                            //if (recmoteControl) SendS10F3ToEquipment(secsGem, $"Panelid: {panelsn},ModelName:{modelName},Linked recipe:{linkedRecipeName}, Current recipe: {machinerecipename + ".recipe"}, Start to change recipe.");

                            if (autoCheckRecipe)
                            {
                                if (MainForm.Instance.ConfirmMessageBox($"Panelid: {panelsn},ModelName:{modelName},recipe 不匹配，是否切换到 {linkedRecipeName} ？"))
                                {
                                    if (!OnPpSelectStatus)//避免重复切换
                                    {
                                        traLog.Info($"Send STOP COMMAND '{linkedRecipeName}'");
                                        var s2f41stop = new SecsMessage(2, 41)
                                        {
                                            SecsItem = L(
                                              A("STOP"),
                                              L(

                                                  )
                                              )
                                        };

                                        var s2f42stop = await secsGem.SendAsync(s2f41stop);
                                        if (s2f42stop.SecsItem.Items[0].FirstValue<byte>() != 0)
                                        {
                                            traLog.Info($"Machine reject stop command");
                                            //SendS10F3ToEquipment(secsGem, $"Send STOP Fail, Code: {s2f42stop.SecsItem.Items[0].FirstValue<byte>()}");
                                            return;
                                        }
                                        else
                                        {
                                            OnPpSelectStatus = true;
                                            OnPpSelectStatusRecipeName = linkedRecipeName;
                                            OnPpSelectStatusTime = DateTime.Now;
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (autoCheckRecipe)
                            {
                                SendPanelStartCommand(secsGem);
                            }
                            traLog.Info($"Panelid: {panelsn} OK");
                        }
                    }
                }
                else
                {
                    traLog.Error($"BarcodeScanned: {baymaxTrans.BaymaxResponse}");
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex);
            }
        }

        internal string? GetAoiRelatedRecipe(ISecsGem secsGem, string recipeGroupName)
        {
            SecsMessage s7f19 = new(7, 19, true)
            {
            };
            var rep = secsGem.SendAsync(s7f19).Result;
            List<string> EPPD = new List<string>();//2103-19010X-XXT.recipe
            foreach (var item in rep.SecsItem.Items)
            {
                EPPD.Add(item.GetString());
            }
            var relatedRecipe = EPPD.FirstOrDefault(it => it.Substring(0, it.Length > 9 ? 9 : it.Length) == recipeGroupName.Substring(0, 9));
            return relatedRecipe;
        }

        private void SendPanelStartCommand(ISecsGem secs)
        {
            //TODO: Send S2F41 BYPASS-CIRCUIT
            var s2f41baypass = new SecsMessage(2, 41)
            {
                SecsItem = L(A("BYPASS-CIRCUIT"), L(A("CIRCUIT"), U4()))
            };
            var s2f42baypass = secs.SendAsync(s2f41baypass).Result;
            if (s2f42baypass.SecsItem.Items[0].FirstValue<byte>() != 0)
            {
                SendS10F3ToEquipment(secs, $"BYPASS-CIRCUIT Fail, Code: {s2f42baypass.SecsItem.Items[0].FirstValue<byte>()}");
                return;
            }
            else
            {
                //TODO: Send S2F41 LOAD
                var s2f41load = new SecsMessage(2, 41)
                {
                    SecsItem = L(A("LOAD"), L())
                };
                secs.SendAsync(s2f41load);
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
