using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using Secs4Net;
using static Secs4Net.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using log4net;
using EAP.Client.Sfis;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class BarcodeScanned : IEventHandler
    {
        private readonly ILog traLog = LogManager.GetLogger("Trace");

        private readonly ISecsGem secsGem;
        private RabbitMqService rabbitMqService;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;

        public BarcodeScanned(RabbitMqService rabbitMq, ISecsGem secsGem, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.rabbitMqService = rabbitMq;
            this.secsGem = secsGem;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;

        }
        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {

            try
            {
                var panelsn = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();
                //var machinerecipename = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();

                SecsMessage s1f3 = new(1, 3, true)
                {
                    SecsItem = L(U4(10100)
                )
                };
                var s1f4 = secsGem.SendAsync(s1f3).Result;
                var machinerecipename = s1f4.SecsItem[0].GetString();

                MainForm.Instance.UpdateMachineRecipe(machinerecipename);

                string equipmentId = configuration.GetSection("Custom")["EquipmentId"];
                string sfisIp = configuration.GetSection("Custom")["SfisIp"];
                int sfisPort = Convert.ToInt32(configuration.GetSection("Custom")["SfisPort"]);
                //string rmsApiUrl = configuration.GetSection("Custom")["RmsApiUrl"];

                traLog.Info($"BarcodeScanned: {panelsn},{machinerecipename}");
                // Send message to SFIS to get LOT INFO

                BaymaxService service = new BaymaxService();
                var trans = await service.GetBaymaxTrans(sfisIp, sfisPort, $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_INFO=???");
                //(bool success, string sfisResponse, string errorMessage) = SendMessageToSfis(sfisIp, sfisPort, $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_INFO=???");
                bool recmoteControl = configuration.GetSection("Custom")["RemoteControl"]?.ToUpper() == "TRUE";

                if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                {
                    // Parse SFIS response to get model name and wafer IDs
                    Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                        .Where(keyValueArray => keyValueArray.Length == 2)
                        .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                    string modelName = sfisParameters["SN_MODEL_NAME_INFO"];
                    MainForm.Instance.UpdateAoiPanelAndModelname(panelsn, modelName);


                    bool autoCheckRecipe = MainForm.Instance.isAutoCheckRecipe;
                    bool autoCheckRecipePara = MainForm.Instance.isAutoCheckRecipePara;
                    if (autoCheckRecipe)
                    {
                        if (!modelName.Contains(machinerecipename.Substring(0, machinerecipename.Length > 9 ? 9 : machinerecipename.Length)))
                        {
                            var errmsg = $"ModelName:{modelName}和Recipe:{machinerecipename}不匹配";
                            traLog.Error(errmsg);
                            MainForm.Instance.ConfirmMessageBox(errmsg);
                            SendS10F3ToEquipment(secsGem, errmsg);
                        }
                        else
                        {
                            if (autoCheckRecipePara)
                            {
                                (bool result, string message) = CheckRecipePara(machinerecipename);
                                if (result)
                                {
                                    traLog.Info(message);
                                }
                                else
                                {
                                    traLog.Error(message);
                                }
                            }
                        }
                    }


                    string LinkedAoiMachine = configuration.GetSection("Custom")["LinkedAoiMachine"];
                    if (!string.IsNullOrEmpty(LinkedAoiMachine))
                    {
                        var rabbitMqtrans = new RabbitMqTransaction
                        {
                            TransactionName = "UpdateOuterSnInfo",
                            EquipmentID = equipmentId
                        };
                        rabbitMqtrans.Parameters.Add("PanelId", panelsn);
                        rabbitMqtrans.Parameters.Add("ModelName", modelName);

                        rabbitMqService.Produce($"EAP.SecsClient.{LinkedAoiMachine}", rabbitMqtrans);
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

        private (bool result, string message) CheckRecipePara(string recipename)
        {
            bool result = false;
            string message = string.Empty;
            try
            {
                var EquipmentId = configuration.GetSection("Custom")["EquipmentId"];
                var trans = new RabbitMqTransaction
                {
                    TransactionName = "CompareRecipeBody",
                    EquipmentID = EquipmentId,
                    NeedReply = true,
                    ExpireSecond = 5,
                    Parameters = new Dictionary<string, object>() { { "EquipmentId", EquipmentId }, { "RecipeName", recipename } }
                };
                var rabbitRes = rabbitMqService.ProduceWaitReply("Rms.Service", trans);
                if (rabbitRes != null)
                {
                    result = rabbitRes.Parameters["Result"].ToString().ToUpper() == "TRUE";
                    rabbitRes.Parameters.TryGetValue("Message", out object messageObj);
                    message = messageObj.ToString();
                }
                else
                {
                    result = false;
                    message = "RabbitMq Trans CompareRecipeBody Time out";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;

            }

            return (result, message);
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
