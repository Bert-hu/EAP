using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using EAP.Client.Utils;
using log4net;
using Microsoft.Extensions.Configuration;
using Secs4Net;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    class CassetteIdReport : IEventHandler
    {
        internal readonly ILog traLog = LogManager.GetLogger("Trace");

        internal readonly ISecsGem secsGem;
        internal readonly IConfiguration configuration;
        public static string nextLot = string.Empty;
        public static string nextRecipe = string.Empty;
        public CassetteIdReport(SecsGem secsGem, IConfiguration configuration)
        {
            this.secsGem = secsGem;
            this.configuration = configuration;
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            traLog.Info($"1.Clamp Success.");
            traLog.Info($"2.Lotid Check Start.");
            uint portid = wrapper.PrimaryMessage.SecsItem[2][0][1][0].FirstValue<uint>();
            string nextLot = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();

            var eqid = configuration.GetSection("Custom")["EquipmentId"];
            var baymaxIp = configuration.GetSection("Custom")["SfisIp"] ?? "10.5.1.226";
            var baymaxPort = int.Parse(configuration.GetSection("Custom")["SfisPort"] ?? "21347");
            string step1Req = $"{eqid},{nextLot},1,M068397,JORDAN,,OK,";
            BaymaxService service = new BaymaxService();
            var step1Trans = service.GetBaymaxTrans(baymaxIp, baymaxPort, step1Req);
            if (step1Trans.Result && step1Trans.BaymaxResponse.ToUpper().StartsWith("OK"))
            {
                var step7Req = $"{eqid},{nextLot},7,M068397,JORDAN,,OK,MODEL_NAME=???";
                var step7Trans = service.GetBaymaxTrans(baymaxIp, baymaxPort, step7Req);
                if (step7Trans.Result && step7Trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                {
                    Dictionary<string, string> sfispara = step7Trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
.Where(keyValueArray => keyValueArray.Length == 2)
.ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                    var modelName = sfispara["MODEL_NAME"];
                    var rmsApiUrl = configuration.GetSection("Custom")["RmsApiUrl"];
                    string url = rmsApiUrl.TrimEnd('/') + "/api/checkrecipegroup";
                    var req = new
                    {
                        EquipmentId = eqid,
                        RecipeGroupName = modelName,
                    };

                    var recipeRes = await HttpClientHelper.HttpPostRequestAsync<CheckRecipeGroupResponse>(url, req);
                    if (recipeRes == null || !recipeRes.Result)
                    {
                        traLog.Error($"Get RecipeGroup Fail: {recipeRes?.Message}");
                        SendS10F3ToEquipment(secsGem, $"Get RecipeGroup Fail: {recipeRes?.Message}");
                        return;
                    }

                    nextRecipe = recipeRes.RecipeName;
                    var s2f41G200 = new SecsMessage(2, 41)
                    {
                        SecsItem = L(
                            A("G200"),
                            L(
                                L(
                                    A("GRDDNY"),
                                    U4(1)
                                ),
                                 L(
                                    A("PORT-ID"),
                                    U4(portid)
                                )
                            )

                        )
                    };
                    await secsGem.SendAsync(s2f41G200);
                }

            }
            else
            {
                traLog.Error($"SFIS FAIL: {step1Trans.BaymaxResponse}");
                SendS10F3ToEquipment(secsGem, $"SFIS FAIL: {step1Trans.BaymaxResponse}");
            }
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

        private class CheckRecipeGroupResponse
        {
            public bool Result { get; set; }
            public string RecipeName { get; set; }
            public string Message { get; set; }
        }
    }
}
