using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using EAP.Client.Sfis;
using log4net;
using Microsoft.Extensions.Configuration;
using Secs4Net;
using System.Windows.Forms;
using static Secs4Net.Item;

namespace EAP.Client.Secs.PrimaryMessageHandler.EventHandler
{
    internal class Strip_2D_Report : IEventHandler
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");
        internal static ILog dbgLog = LogManager.GetLogger("Debug");

        private RabbitMqService rabbitMqService;
        private readonly ISecsGem secsGem;
        private readonly CommonLibrary commonLibrary;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;

        public Strip_2D_Report(RabbitMqService rabbitMqService, ISecsGem secsGem, CommonLibrary commonLibrary, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.rabbitMqService = rabbitMqService;
            this.secsGem = secsGem;
            this.commonLibrary = commonLibrary;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
        }

        public async Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper)
        {
            var checkResult = "OK";
            string errMsg = string.Empty;

            try
            {
                var panelid = wrapper.PrimaryMessage.SecsItem[2][0][1][1].GetString();


                if (MainForm.Instance.isAutoCheckRecipe)
                {
                    uint ppidSv = 2002;
                    var s1f3 = new SecsMessage(1, 3)
                    {
                        SecsItem = L(U4(ppidSv))
                    };
                    var s1f4 = secsGem.SendAsync(s1f3).Result;
                    var recipeName = s1f4.SecsItem.Items[0].GetString();

                    string sfisIp = commonLibrary.CustomSettings["SfisIp"];
                    int sfisPort = Convert.ToInt32(commonLibrary.CustomSettings["SfisPort"]);
                    var getModelProjextReq = $"EQXXXXXX01,{panelid},7,M001603,JORDAN,,OK,SN_MODEL_NAME_PROJECT_NAME_INFO=???";
                    BaymaxService baymax = new BaymaxService();
                    var result = await baymax.GetBaymaxTrans(sfisIp, sfisPort, getModelProjextReq);
                    if (result.Result && result.BaymaxResponse.ToUpper().StartsWith("OK"))
                    {
                        Dictionary<string, string> sfisParameters = result.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
              .Where(keyValueArray => keyValueArray.Length == 2)
              .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                        string modelName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[0];
                        string projectName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[1];
                        MainForm.Instance.UpdateProductInfo(panelid, recipeName, modelName,projectName);

                        var case1 = modelName.Substring(5, 4);
                        var case2 = modelName.Substring(7, 4);
                        if (recipeName.Contains(case1) || recipeName.Contains(case2))
                        {
                            string equipmentId = configuration.GetSection("Custom")["EquipmentId"];

                            var rabbitTrans = new RabbitMqTransaction()
                            {
                                TransactionName = "CompareRecipeBody",
                                EquipmentID = equipmentId,
                                Parameters = new Dictionary<string, object>()
                                        {
                                            {"EquipmentId",equipmentId},
                                            {"RecipeName",recipeName},
                                        },
                            };
                            var repTrans = rabbitMqService.ProduceWaitReply("Rms.Service", rabbitTrans);
                            if (repTrans != null)
                            {
                                var bodyResult = false;
                                var message = string.Empty;
                                if (repTrans.Parameters.TryGetValue("Result", out object _result)) bodyResult = (bool)_result;
                                if (repTrans.Parameters.TryGetValue("Message", out object _message)) message = _message?.ToString();
                                if (!bodyResult)
                                {
                                    checkResult = "NG";
                                    traLog.Error("程式参数比较失败: " + message);
                                }
                                else
                                {
                                    checkResult = "OK";
                                    traLog.Info("程式参数比较成功!");
                                }
                            }
                            else
                            {
                                traLog.Error("程式参数比较失败: Timeout");
                            }
                        }
                        else
                        {
                            checkResult = "NG";
                            errMsg = $"Recipe is not match:{recipeName},{modelName}";
                            traLog.Error("程式参数比较失败: " + $"Recipe is not match:{recipeName},{modelName}");
                        }

                    }
                    else
                    {
                        checkResult = "NG";
                        errMsg = $"SFIS Get ModelName Error";
                    }
                }


            }
            catch (Exception ex)
            {
                traLog.Error(ex.ToString());
            }
            if (checkResult != "OK")
            {
                SecsMessage s10f3 = new(10, 3, false)
                {
                    SecsItem = L(B(0x00), A(errMsg))
                };
                secsGem.SendAsync(s10f3);
            }
            var s2f41 = new SecsMessage(2, 41)
            {
                SecsItem = L(A("STRIP_2D_CHECK"), L(L(A("CheckResult"), A(checkResult))))
            };
            secsGem.SendAsync(s2f41);
        }
    }
}
