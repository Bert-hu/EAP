using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Sfis;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EAP.Client.Service
{
    public class BesiMoldingService
    {
        internal static ILog dbgLog = LogManager.GetLogger("Debug");

        private readonly IConfiguration configuration;
        private readonly RabbitMqService rabbitMqService;
        public BesiMoldingService(IConfiguration configuration, RabbitMqService rabbitMqService)
        {
            this.configuration = configuration;
            this.rabbitMqService = rabbitMqService;
        }
        public async Task<(string? modelName, string? errMsg)> GetModelNameBySn(string sn)
        {
            var site = configuration.GetSection("Custom")["Site"] ?? "HPH";
            var sfisIp = configuration.GetSection("Custom")["SfisIp"];
            var sfisPort = int.Parse(configuration.GetSection("Custom")["SfisPort"] ?? "21347");
            //var equipmentId = configuration.GetSection("Custom")["EquipmentId"];
            var getModelnameReq = string.Empty;
            if (site == "JQ")
            {
                getModelnameReq = $"SMD_SPC_QUERY,{sn},7,M090696,JQ01-3FAP-12,,OK,SN_MODEL_NAME_PROJECT_NAME_INFO=???";//JQ
            }
            else
            {
                getModelnameReq = $"EQXXXXXX01,{sn},7,M001603,V98,,OK,SN_MODEL_NAME_INFO=???";//HPH
            }

            //var getModelnameRes = string.Empty;
            //var getModelnameErr = string.Empty;
            string recipeName = null;
            string modelname = null;
            string errMsg = null;

            BaymaxService baymax = new BaymaxService();
            var trans = await baymax.GetBaymaxTrans(sfisIp, sfisPort, getModelnameReq);

            if (trans.Result)
            {
                if (trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                {
                    Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                                  .Where(keyValueArray => keyValueArray.Length == 2)
                                  .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);

                    if (site == "JQ")
                    {
                        //JQ
                        modelname = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[0];
                        //string projectName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[1];
                        //string groupName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[2];
                    }
                    else
                    {
                        //HPH
                        modelname = sfisParameters["SN_MODEL_NAME_INFO"];
                    }
                    return (modelname, null);
                }
                else
                {
                    return (modelname, "SFIS Fail: " + trans.BaymaxResponse);
                }
            }
            else
            {
                return (modelname, trans.BaymaxResponse);
            }

        }

        public async Task<(List<string>? alias, string errMsg)> GetRecipeNameAlias(string recipeName)
        {
            var message = string.Empty;
            try
            {
                var equipmentTypeId = configuration.GetSection("Custom")["EquipmentType"];
                var trans = new RabbitMqTransaction
                {
                    TransactionName = "GetRecipeNameAlias",
                    ExpireSecond = 3,
                    NeedReply = true,
                    Parameters = new Dictionary<string, object>
                    {
                        { "RecipeName", recipeName },
                        { "EquipmentTypeId", equipmentTypeId}
                    }
                };
                var repTrans = rabbitMqService.ProduceWaitReply("Rms.Service", trans);
                if (repTrans != null)
                {
                    var result = false;
                    if (repTrans.Parameters.TryGetValue("Result", out object _result)) result = (bool)_result;
                    if (repTrans.Parameters.TryGetValue("Message", out object _message)) message = _message?.ToString();
                    if (!result)
                    {
                        message = "GetRecipeNameAlias Fail: " + message;
                    }
                    else
                    {
                        var alias = JsonConvert.DeserializeObject<List<string>>(repTrans.Parameters["RecipeAlias"].ToString());
                        return (alias, message);
                    }
                }
                else
                {
                    message = "GetRecipeNameAlias Fail: No reply from Rms.Service";
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error($"GetRecipeNameByAlias Error: {ex.Message}", ex);
                message = $"GetRecipeNameByAlias Error: {ex.Message}";
            }
            return (null, message);
        }

        public async Task<(string materialPn, string errMsg)> GetMaterialPn(string reelId)
        {
            string message = string.Empty;
            try
            {
                var sfisIp = configuration.GetSection("Custom")["SfisIp"];
                var sfisPort = int.Parse(configuration.GetSection("Custom")["SfisPort"] ?? "21347");

                //TODO: 等IT文档出来再修改
                var getPnReq = $"SMD_SPC_QUERY,{reelId},7,M090696,JQ01-3FAP-12,,OK,MaterialInfo=???";//JQ
                BaymaxService baymax = new BaymaxService();
                var trans = await baymax.GetBaymaxTrans(sfisIp, sfisPort, getPnReq);

                if (trans.Result)
                {
                    if (trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                    {
                        Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                                      .Where(keyValueArray => keyValueArray.Length == 2)
                                      .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);


                        //TODO: 等IT文档出来再修改
                        string materialPn = sfisParameters["MaterialInfo"].TrimEnd(';').Split(':')[0];
                        return (materialPn, message);
                    }
                    else
                    {
                        return (string.Empty, "SFIS Fail: " + trans.BaymaxResponse);
                    }
                }
                else
                {
                    return (string.Empty, trans.BaymaxResponse);
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error($"GetMaterialPn Error: {ex.Message}", ex);
            }
            return (string.Empty, message);
        }

    }
}
