using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
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
                var (recipeName, modelName, recipeErrorMessage) = await MainForm.Instance.GetRecipeNameBySn(panelId);

                if (recipeName == null)
                {
                    HandleValidationFailure($"Fail, Check Model Name Fail: {recipeErrorMessage}");
                    return;
                }

                if (recipeName != machineRecipeName)
                {
                    HandleValidationFailure($"Fail, Recipe is not match. ModelName:{modelName}, Linked recipe:{recipeName}, Machine recipe: {machineRecipeName}");
                    return;
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
        }


        private bool ValidateRecipeBody(out string compareBodyMessage)
        {
            compareBodyMessage = "Do not compare recipe body";

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
