using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using Secs4Net;
using Sunny.UI;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class StripValidationRequest : IEventHandler
    {

        private RabbitMqService rabbitMqService;
        private readonly ISecsGem secsGem;
        private readonly CommonLibrary commonLibrary;
        private readonly IServiceProvider serviceProvider;

        public StripValidationRequest(RabbitMqService rabbitMqService, ISecsGem secsGem, CommonLibrary commonLibrary, IServiceProvider serviceProvider)
        {
            this.rabbitMqService = rabbitMqService;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
            this.serviceProvider = serviceProvider;
        }

        public Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            var panelsn = wrapper.PrimaryMessage.SecsItem[2][0][1][0].GetString();
            var machinerecipename = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();
            return null;
        //    if (string.IsNullOrEmpty(MainForm.Instance.uiTextBox_reelId.Text))
        //    {
        //        HandleValidationFailure("Panel In: Fail, ReelID is null.");
        //        return;
        //    }

        //    if (MainForm.Instance.isAutoCheckRecipeName)
        //    {
        //        var (recipeName, modelName, recipeErrorMessage) = GetRecipeNameBySn(panelId);

        //        if (recipeName == null)
        //        {
        //            HandleValidationFailure($"Fail, Check Model Name Fail: {recipeErrorMessage}");
        //            return;
        //        }

        //        if (recipeName != machineRecipe)
        //        {
        //            HandleValidationFailure($"Fail, Recipe is not match. ModelName:{modelName}, Linked recipe:{recipeName}, Machine recipe: {machineRecipe}");
        //            return;
        //        }
        //    }

        //    if (!ValidateRecipeBody(out string compareBodyMessage))
        //    {
        //        HandleValidationFailure(compareBodyMessage);
        //        return;
        //    }

        //    // 直接在这里构建 panelIn 消息
        //    var panelIn = $"{variables.Station},{panelId},2,{variables.EMP},{variables.Line},,OK,,,ACTUAL_GROUP={variables.Group},,,,,,{variables.ModelName},{variables.ReelID}";
        //    var trans = baymax.GetBaymaxTrans(variables.SfisIp, variables.SfisPort, panelIn);

        //    if (!trans.Result || !trans.BaymaxResponse.ToUpper().StartsWith("OK"))
        //    {
        //        HandleValidationFailure(trans.BaymaxResponse);
        //        return;
        //    }

        //    SendValidationResponse(true);
        }
    }
}
