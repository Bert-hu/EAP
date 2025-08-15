using EAP.Client.Forms;
using EAP.Client.Models;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Service;
using EAP.Client.Sfis;
using EAP.Client.Utils;
using log4net;
using Microsoft.Extensions.Configuration;
using Secs4Net;
using Sunny.UI;
using System.Configuration;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class StripValidationRequest : IEventHandler
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");


        private RabbitMqService rabbitMqService;
        private readonly ISecsGem secsGem;
        private readonly CommonLibrary commonLibrary;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;

        public StripValidationRequest(RabbitMqService rabbitMqService, ISecsGem secsGem, CommonLibrary commonLibrary, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.rabbitMqService = rabbitMqService;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            var panelId = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();
            var machineRecipeName = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();
            MainForm.Instance?.UpdateMachineRecipe(machineRecipeName);
            if (string.IsNullOrEmpty(MainForm.Instance.uiTextBox_reelId.Text))
            {
                HandleValidationFailure("Panel In: Fail, ReelID is null.");
                return;
            }

            if (MainForm.Instance.checkBox_checkrecipe.Checked)
            {
                string message = string.Empty;
                ConfigManager<MoldingConfig> manager = new ConfigManager<MoldingConfig>();
                var config = manager.LoadConfig();

                BesiMoldingService besiMolding = new BesiMoldingService(configuration, rabbitMqService);

                //string modelName = string.Empty;
                //(modelName, message)  = await besiMolding.GetModelNameBySn(panelId);
                //if (string.IsNullOrEmpty(modelName))
                //{
                //    HandleValidationFailure($"Panel In: Fail, 获取{panelId}的ModelName失败: {message}");
                //    return;
                //}
                //MainForm.Instance.UpdateModelName(modelName);

                string materialPn = string.Empty;
                (materialPn, message) = await besiMolding.GetMaterialPn(config.ReelId);
                if (string.IsNullOrEmpty(materialPn))
                {
                    HandleValidationFailure($"Panel In: Fail, 获取{config.ReelId}的PN失败: {message}");
                    return;
                }

                List<string>? recipeAlias = null;
                (recipeAlias, message) = await besiMolding.GetRecipeNameAlias(machineRecipeName);
                if (recipeAlias == null || recipeAlias.Count == 0)
                {
                    HandleValidationFailure($"Panel In: Fail, 获取{machineRecipeName}绑定的PN List失败: {message}");
                    return;
                }
                else
                {
                    if (recipeAlias.Contains(materialPn))
                    {
                        traLog.Info($"{machineRecipeName}绑定的PN列表包含{materialPn}，RecipeName检查通过");
                    }
                    else
                    {
                        HandleValidationFailure($"Panel In: Fail, 绑定的PN列表({string.Join(",", recipeAlias)})不包含{materialPn}，RecipeName检查失败");
                        return;
                    }
                }
            }

            if (!ValidateRecipeBody(out string compareBodyMessage))
            {
                HandleValidationFailure(compareBodyMessage);
                return;
            }

            var equipmentId = configuration.GetSection("Custom")["EquipmentId"];

            // 直接在这里构建 panelIn 消息
            var panelIn = $"{equipmentId},{panelId},2,{MainForm.Instance.uiTextBox_empNo.Text},{MainForm.Instance.uiTextBox_line.Text},,OK,,,ACTUAL_GROUP={MainForm.Instance.uiTextBox_groupName.Text},,,,,,{MainForm.Instance.uiTextBox_modelName.Text},{MainForm.Instance.uiTextBox_reelId.Text}";

            var sfisIp = configuration.GetSection("Custom")["SfisIp"];
            var sfisPort = int.Parse(configuration.GetSection("Custom")["SfisPort"] ?? "21347");


            BaymaxService baymax = new BaymaxService();
            var trans = await baymax.GetBaymaxTrans(sfisIp, sfisPort, panelIn);

            if (!trans.Result || !trans.BaymaxResponse.ToUpper().StartsWith("OK"))
            {
                HandleValidationFailure(trans.BaymaxResponse);
                return;
            }

            SendValidationResponse(true);
            traLog.Info($"{panelId}检查全部通过，允许进板。");
        }


        private bool ValidateRecipeBody(out string compareBodyMessage)
        {
            compareBodyMessage = "未勾选比较Recipe Body";

            if (!MainForm.Instance.uiCheckBox_checkRecipeBody.Checked)
            {
                traLog.Info(compareBodyMessage);
                return true;
            }

            var (compareBodyResult, bodyMessage) = CheckRecipePara(MainForm.Instance.textBox_machinerecipe.Text);
            compareBodyMessage = bodyMessage;

            if (!compareBodyResult)
            {
                traLog.Error("Compare recipe body fail");
                traLog.Error(bodyMessage);
            }

            return compareBodyResult;
        }

        private void HandleValidationFailure(string errorMsg)
        {
            SendValidationResponse(false);
            SendS10F3ToEquipment(secsGem, errorMsg);
            traLog.Error(errorMsg);
        }

        private void SendValidationResponse(bool isValid)
        {
            var status = isValid ? "ACCEPT" : "REJECT";
            SecsMessage s2f41 = new(2, 41, true)
            {
                SecsItem = L(
                    A("STRIPVALIDATION"),
                    L(
                        L(
                            A("STRIPSTATUS"),
                            A(status)
                        )
                    )
                )
            };
            secsGem.SendAsync(s2f41);
        }

        private (bool result, string message) CheckRecipePara(string recipename)
        {
            bool result = false;
            string message = string.Empty;
            try
            {
                var EquipmentId = commonLibrary.CustomSettings["EquipmentId"];
                var trans = new RabbitMqTransaction
                {
                    TransactionName = "CompareRecipeBody",
                    EquipmentID = EquipmentId,
                    NeedReply = true,
                    ExpireSecond = 3,
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
