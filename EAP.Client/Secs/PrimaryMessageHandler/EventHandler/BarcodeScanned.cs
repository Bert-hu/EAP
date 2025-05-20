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
        private static readonly log4net.ILog dbgLog = log4net.LogManager.GetLogger("Debug");
        private static readonly log4net.ILog traLog = log4net.LogManager.GetLogger("Trace");

        private readonly ISecsGem secsGem;
        private readonly RabbitMqService rabbitMqService;
        private readonly CommonLibrary commonLibrary;

        //public static bool OnPpSelectStatus { get; set; } = false;
        //public static string OnPpSelectStatusRecipeName { get; set; } = string.Empty;
        //public static DateTime OnPpSelectStatusTime { get; set; } = DateTime.MinValue;

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
                //var baymaxTrans = baymax.GetBaymaxTrans(sfisIp, sfisPort, $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_INFO=???").Result;
                var baymaxTrans = baymax.GetBaymaxTrans(sfisIp, sfisPort, $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_PROJECT_NAME_INFO=???").Result;
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
                        //string modelName = sfisParameters["SN_MODEL_NAME_INFO"];
                        string modelName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[0];
                        string projectName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[1];

                        MainForm.Instance.UpdatePanelInfo(panelsn, modelName,projectName);

                        //(string linkedRecipeName, string recipeErrorMessage) = CheckRecipeGroup(rmsApiUrl, equipmentId, modelName);
                        string linkedRecipeName = GetAoiRelatedRecipe(secsGem, modelName);

                        if (linkedRecipeName == null)
                        {
                            if (autoCheckRecipe)
                            {
                                MainForm.Instance.ConfirmMessageBox("未找到关联的recipe");
                            }
                        }
                        else if (linkedRecipeName != machinerecipename )
                        {
                            traLog.Warn($"Panelid: {panelsn},ModelName:{modelName},recipe mismatch: {linkedRecipeName} != {machinerecipename + ".recipe"}");


                            //if (autoCheckRecipe)
                            //{
                            //    if (MainForm.Instance.ConfirmMessageBox($"Panelid: {panelsn},ModelName:{modelName},recipe 不匹配，是否切换到 {linkedRecipeName} ？"))
                            //    {
                            //        if (!ProcessStateChanged.OnPpSelectStatus)//避免重复切换
                            //        {
                            //            traLog.Info($"Send STOP COMMAND '{linkedRecipeName}'");
                            //            var s2f41stop = new SecsMessage(2, 41)
                            //            {
                            //                SecsItem = L(
                            //                  A("STOP"),
                            //                  L(

                            //                      )
                            //                  )
                            //            };

                            //            var s2f42stop = await secsGem.SendAsync(s2f41stop);
                            //            if (s2f42stop.SecsItem.Items[0].FirstValue<byte>() != 0)
                            //            {
                            //                traLog.Info($"Machine reject stop command");
                            //                //SendS10F3ToEquipment(secsGem, $"Send STOP Fail, Code: {s2f42stop.SecsItem.Items[0].FirstValue<byte>()}");
                            //                return;
                            //            }
                            //            else
                            //            {
                            //                ProcessStateChanged.OnPpSelectStatus = true;
                            //                ProcessStateChanged.ChangeRecipeName = linkedRecipeName;
                            //                ProcessStateChanged.ChangeDateTime = DateTime.Now;
                            //            }
                            //        }
                            //    }
                            //}

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
