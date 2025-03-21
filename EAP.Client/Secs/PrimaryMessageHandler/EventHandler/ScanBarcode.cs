using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using EAP.Client.Utils;
using log4net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Secs4Net;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Secs4Net.Item;
using EAP.Client.Forms;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class ScanBarcode : IEventHandler
    {
        private ILog traLog = LogManager.GetLogger("Trace");

        private readonly ISecsGem secsGem;
        private readonly RabbitMqService rabbitMqService;
        private readonly IServiceProvider serviceProvider;
        private readonly CommonLibrary commonLibrary;

        public ScanBarcode(RabbitMqService rabbitMq, ISecsGem secsGem, IServiceProvider serviceProvider, CommonLibrary commonLibrary)
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
                // Get lot ID
                string lotId = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();

                // Get equipment ID, SFIS IP, SFIS port and RMS API URL from configuration
                string equipmentId = commonLibrary.CustomSettings["EquipmentId"];
                string sfisIp = commonLibrary.CustomSettings["SfisIp"];
                int sfisPort = Convert.ToInt32(commonLibrary.CustomSettings["SfisPort"]);
                string rmsApiUrl = commonLibrary.CustomSettings["RmsApiUrl"];

                // Send message to SFIS to get LOT INFO
                //(bool success, string sfisResponse, string errorMessage) = await SendMessageToSfisAsync(sfisIp, sfisPort, $"{equipmentId},{lotId},7,M068397,JORDAN,,OK,MODEL_NAME=???  WAFER_IDS=??? VENDER_LOT=???");
                BaymaxService service = new BaymaxService();
                var trans = service.GetBaymaxTrans(sfisIp, sfisPort, $"{equipmentId},{lotId},7,M068397,JORDAN,,OK,MODEL_NAME=???  WAFER_IDS=??? VENDER_LOT=???");

                if (trans.Result)
                {
                    if (trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                    {
                        // Parse SFIS response to get model name and wafer IDs
                        Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                            .Where(keyValueArray => keyValueArray.Length == 2)
                            .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);

                        string modelName = sfisParameters["MODEL_NAME"];
                        //string waferIds = sfisParameters["WAFER_IDS"];
                        //string vendorLot = sfisParameters["VENDER_LOT"];

                        // Check recipe group
                        //(string recipeName, string recipeErrorMessage) = await CheckRecipeGroupAsync(rmsApiUrl, equipmentId, modelName);
                        MainForm.Instance.UpdateLotAndModelname(lotId, modelName);

                        string url = rmsApiUrl.TrimEnd('/') + "/api/checkrecipegroup";
                        var req = new
                        {
                            EquipmentId = equipmentId,
                            RecipeGroupName = modelName,
                        };

                        var recipeRes = await HttpClientHelper.HttpPostRequestAsync<CheckRecipeGroupResponse>(url, req);



                        if (recipeRes == null || !recipeRes.Result)
                        {
                            traLog.Debug($"Get RecipeGroup Fail: {recipeRes?.Message}");
                            SendS10F3ToEquipment(secsGem, $"Get RecipeGroup Fail: {recipeRes?.Message}");
                            return;
                        }


                        // Get marking texts
                        traLog.Debug($"Get Marking Content Start.");
                        (string[] markingTexts, string markingErrorMessage) = await GetMarkingTextsAsync(rmsApiUrl, equipmentId, recipeRes.RecipeName, sfisParameters);
                        if (!string.IsNullOrEmpty(markingErrorMessage))
                        {
                            traLog.Error($"Get Marking Content Fail.");
                            traLog.Error(markingErrorMessage);
                            SendS10F3ToEquipment(secsGem, markingErrorMessage);
                            return;
                        }
                        traLog.Debug($"Get Marking Content Success.");

                        // Send PP-SELECT message to equipment
                        traLog.Debug($"Send PP-SELECT Start.");
                        List<Item> parameterList = new List<Item>
                {
                    L(A("LOT ID"), A(lotId)),
                    L(A("RECIPE NAME"), A(recipeRes.RecipeName))
                };
                        for (int i = 0; i < markingTexts.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(markingTexts[i]))
                            {
                                parameterList.Add(L(A($"TEXT{i + 1}"), A(markingTexts[i])));
                            }
                        }
                        SecsMessage s2f41 = new(2, 41)
                        {
                            SecsItem = L(
                                A("PP-SELECT"),
                                L(parameterList))
                        };
                        _ = secsGem.SendAsync(s2f41);
                        traLog.Debug($"Send PP-SELECT Success.");
                        traLog.Debug($"Lot ({lotId}) Start: {string.Join(",", markingTexts)})");
                        MainForm.Instance.UpdateMachineRecipe(recipeRes.RecipeName);
                    }
                }
                else
                {
                    // Send error message to equipment
                    SendS10F3ToEquipment(secsGem, $"EAP FAIL: {trans.BaymaxResponse}");
                }
            }
            catch (Exception ex)
            {
                // Log exception and send error message to equipment
                traLog.Error($"An exception occurred: {ex}");
                SendS10F3ToEquipment(secsGem, $"An exception occurred: {ex.Message}");
            }
        }

        private class CheckRecipeGroupResponse
        {
            public bool Result { get; set; }
            public string RecipeName { get; set; }
            public string Message { get; set; }
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

        internal async Task<(string[] markingTexts, string errMsg)> GetMarkingTextsAsync(string rmsUrl, string equipmentId, string recipeName, Dictionary<string, string> sfisParameter)
        {
            string[] markingTexts = null;
            string errMsg = string.Empty;

            try
            {
                string url = rmsUrl.TrimEnd('/') + "/api/getmarkingtexts";
                var req = new
                {
                    TrueName = "EAP",
                    EquipmentId = equipmentId,
                    RecipeName = recipeName,
                    SfisParameter = sfisParameter
                };
                var reqstr = JsonConvert.SerializeObject(req);
                using (var httpClient = new HttpClient())
                {
                    var httpResponse = await httpClient.PostAsync(url, new StringContent(reqstr, Encoding.UTF8, "application/json"));
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                        var repobj = JObject.Parse(responseContent);
                        if ((bool)repobj["Result"])
                        {
                            markingTexts = repobj["MarkingTexts"].Select(it => it.ToString()).ToArray();
                            return (markingTexts, errMsg);
                        }
                        else
                        {
                            errMsg = repobj["Message"].ToString();
                        }
                    }
                    else
                    {
                        errMsg = $"HTTP请求失败，状态码：{httpResponse.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = $"发生异常：{ex.Message}";
            }

            return (markingTexts, errMsg);
        }

    }
}
