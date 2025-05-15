using EAP.Client.Secs;
using log4net;
using Newtonsoft.Json;
using Secs4Net;
using System.Text;
using static Secs4Net.Item;

namespace EAP.Client.RabbitMq
{
    internal class CompareRecipe : ITransactionHandler
    {
        internal readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;
        internal readonly ISecsConnection hsmsConnection;
        internal readonly CommonLibrary commonLibrary;

        public CompareRecipe(RabbitMqService rabbitMq, ISecsGem secsGem, ISecsConnection hsmsConnection, CommonLibrary commonLibrary)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
            this.hsmsConnection = hsmsConnection;
            this.commonLibrary = commonLibrary;


        }

        List<string> ItemDic = new List<string>
        {
            "DeGAS_TIME_6001",
            "Degas Inner Temp_6002",
            "Degas Outter Temp_6003",
            "RF_POWER_1st_6004",
            "RF_clean_time_1st_6005",
            "RF_pumpdown_Time_1st_6006",
            "RF_MFC1_O2_Flow_1st_6007",
            "RF_MFC2_N2_Flow_1st_6008",
            "RF_MFC3_Ar_Flow_1st_6009",
            "RF_POWER_2nd_6010",
            "RF_clean_time_2nd_6011",
            "RF_pumpdown_Time_2nd_6012",
            "RF_MFC1_O2_Flow_2nd_6013",
            "RF_MFC2_N2_Flow_2nd_6014",
            "RF_MFC3_Ar_Flow_2nd_6015",
            "RF_POWER_3rd_6016",
            "RF_clean_time_3rd_6017",
            "RF_pumpdown_Time_3rd_6018",
            "RF_MFC1_O2_Flow_3rd_6019",
            "RF_MFC2_N2_Flow_3rd_6020",
            "RF_MFC3_Ar_Flow_3rd_6021",
            "RF_POWER_4th_6022",
            "RF_clean_time_4th_6023",
            "RF_pumpdown_Time_4th_6024",
            "RF_MFC1_O2_Flow_4th_6025",
            "RF_MFC2_N2_Flow_4th_6026",
            "RF_MFC3_Ar_Flow_4th_6027",
            "RF_POWER_5th_6028",
            "RF_clean_time_5th_6029",
            "RF_pumpdown_Time_5th_6030",
            "RF_MFC1_O2_Flow_5th_6031",
            "RF_MFC2_N2_Flow_5th_6032",
            "RF_MFC3_Ar_Flow_5th_6033",
            "RF_POWER_6th_6034",
            "RF_clean_time_6th_6035",
            "RF_pumpdown_Time_6th_6036",
            "RF_MFC1_O2_Flow_6th_6037",
            "RF_MFC2_N2_Flow_6th_6038",
            "RF_MFC3_Ar_Flow_6th_6039",
            "RF_POWER_7th_6040",
            "RF_clean_time_7th_6041",
            "RF_pumpdown_Time_7th_6042",
            "RF_MFC1_O2_Flow_7th_6043",
            "RF_MFC2_N2_Flow_7th_6044",
            "RF_MFC3_Ar_Flow_7th_6045",
            "RF_POWER_8th_6046",
            "RF_clean_time_8th_6047",
            "RF_pumpdown_Time_8th_6048",
            "RF_MFC1_O2_Flow_8th_6049",
            "RF_MFC2_N2_Flow_8th_6050",
            "RF_MFC3_Ar_Flow_8th_6051",
            "RF_POWER_9th_6052",
            "RF_clean_time_9th_6053",
            "RF_pumpdown_Time_9th_6054",
            "RF_MFC1_O2_Flow_9th_6055",
            "RF_MFC2_N2_Flow_9th_6056",
            "RF_MFC3_Ar_Flow_9th_6057",
            "RF_POWER_10th_6058",
            "RF_clean_time_10th_6059",
            "RF_pumpdown_Time_10th_6060",
            "RF_MFC1_O2_Flow_10th_6061",
            "RF_MFC2_N2_Flow_10th_6062",
            "RF_MFC3_Ar_Flow_10th_6063",
            "RF_Times_6064",
            "RF_Bias_Alarm_Voltage_6065",
            "DC_01_SPT_POWER_6066",
            "DC_01_ON_TIME_6067",
            "DC_01_OFF_TIME_6068",
            "DC_02_SPT_POWER_6069",
            "DC_02_ON_TIME_6070",
            "DC_02_OFF_TIME_6071",
            "DC_03_SPT_POWER_6072",
            "DC_03_ON_TIME_6073",
            "DC_03_OFF_TIME_6074",
            "DC_04_SPT_POWER_6075",
            "DC_04_ON_TIME_6076",
            "DC_04_OFF_TIME_6077",
            "DC_05_SPT_POWER_6078",
            "DC_05_ON_TIME_6079",
            "DC_05_OFF_TIME_6080",
            "DC_06_SPT_POWER_6081",
            "DC_06_ON_TIME_6082",
            "DC_06_OFF_TIME_6083",
            "DC_07 SPT_POWER_6084",
            "DC_07 ON TIME_6085",
            "DC_07 OFF TIME_6086",
            "DC_08 SPT_POWER_6087",
            "DC_08 ON TIME_6088",
            "DC_08 OFF TIME_6089",
            "Forw_SPT_Standing_time_6090",
            "Back_SPT_Standing_time_6091",
            "M4_MFC1_Ar_Setting_6092",
            "M6_MFC2_Ar_Setting_6093",
            "M8_MFC3_Ar_Setting_6094",
            "M10_MFC4_Ar_Setting_6095",
            "Line_speed_SPT_speed_6096",
            "FILE_01_NUM_6097",
            "FILE_02_NUM_6098",
            "FILE_03_NUM_6099",
            "FILE_04_NUM_6100",
            "FILE_05_NUM_6101",
            "FILE_06_NUM_6102",
            "FILE_07_NUM_6103",
            "FILE_08_NUM_6104",
            "FILE_09_NUM_6105",
            "FILE_10_NUM_6106",
            "FILE_11_NUM_6107",
            "FILE_12_NUM_6108",
            "COUNTER_01_6109",
            "COUNTER_02_6110",
            "COUNTER_03_6111",
            "COUNTER_04_6112",
            "COUNTER_05_6113",
            "COUNTER_06_6114",
            "COUNTER_07_6115",
            "COUNTER_08_6116",
            "COUNTER_09_6117",
            "COUNTER_10_6118",
            "COUNTER_11_6119",
            "COUNTER_12_6120",
            "POLYCOLD_Enable",
            "ALL_TP_LOW_SPEED_Enable",
            "Program Name"
        };

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var recipename = string.Empty;
                //var recipeParameters = string.Empty;
                string base64String = string.Empty;

                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();
                //if (trans.Parameters.TryGetValue("RecipeParameters", out object _parameters)) recipeParameters = _parameters.ToString();
                if (trans.Parameters.TryGetValue("RecipeBody", out object _body)) base64String = _body.ToString();

                byte[] decodedBytes = Convert.FromBase64String(base64String);
                // 将字节数组转换回字符串
                string smlString = System.Text.Encoding.UTF8.GetString(decodedBytes);

                var serverBody = commonLibrary.StringToSecsMessage(smlString);

                var reqindex = recipename.Split('_')[0];
                SecsMessage s7f25 = new(7, 25, true)
                {
                    SecsItem = A(reqindex)
                };
                var machineBody = await secsGem.SendAsync(s7f25);


                var compareResult = CompareSputterFormattedRecipe(serverBody, serverBody);


                reptrans.Parameters.Add("Result", string.IsNullOrEmpty(compareResult));
                reptrans.Parameters.Add("Message", compareResult);

            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error: {ex.Message}");
                dbgLog.Error(ex.Message, ex);
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }

        Dictionary<int, string> configIndexName = new Dictionary<int, string>()
        {
            { 4, "M3_M5_1"},
            { 5, "M3_M5_2"},
            { 6, "M3_M5_3"},
            { 7, "M5_M7_1"},
            { 8, "M5_M7_2"},
            { 9, "M5_M7_3"},
            { 10, "M7_M9_1"},
            { 11, "M7_M9_2"},
            { 12, "M7_M9_3"},
            { 13, "M9_M11_1"},
            { 14, "M9_M11_2"},
            { 15, "M9_M11_3"},
        };

        public string CompareSputterFormattedRecipe(SecsMessage serverBody, SecsMessage machineBody)
        {
            StringBuilder result = new StringBuilder();
            try
            {

                var serverRecipeIndex = serverBody.SecsItem[0].GetString();
                var machineRecipeIndex = machineBody.SecsItem[0].GetString();
                if (serverRecipeIndex != machineRecipeIndex)
                {
                    return ($"服务器Recipe编号:{serverRecipeIndex} 与 设备Recipe编号:{machineRecipeIndex} 不同");
                }
                else
                {
                    foreach (var config in configIndexName)
                    {
                        var configName = config.Value;
                        var serverConfig = serverBody.SecsItem[config.Key].Items;
                        var machineConfig = machineBody.SecsItem[config.Key].Items;
                        var serverConfigCount = serverConfig.Count();
                        var machineConfigCount = machineConfig.Count();
                        if (serverConfigCount == 0 && machineConfigCount > 0)
                        {
                            result.AppendLine($"{configName}设备有Recipe内容，服务器无Recipe内容。");
                        }
                        else if (serverConfigCount > 0 && machineConfigCount == 0)
                        {
                            result.AppendLine($"{configName}服务器有Recipe内容，设备无Recipe内容。");
                        }
                        else if (serverConfigCount == 0 && machineConfigCount == 0)
                        {
                            //正常情况
                        }
                        else
                        {
                            for (int i = 0; i < serverConfigCount; i++)
                            {
                                var serverItemValue = GetItemValue(serverConfig[i].Format, serverConfig[i]);
                                var machineItemValue = GetItemValue(machineConfig[i].Format, machineConfig[i]);
                                if (serverItemValue != machineItemValue)
                                {
                                    result.AppendLine($"{configName} {ItemDic[i]}不一致，服务器：{serverItemValue}，设备：{machineItemValue}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
                return $"EAP Error: {ex.Message}";
            }
            return result.ToString();
        }

        private string GetItemValue(SecsFormat format, Item? item)
        {
            string value = string.Empty;
            switch (format)
            {
                case SecsFormat.ASCII:
                    value = item.GetString();
                    break;
                case SecsFormat.I1:
                    value = item.FirstValue<sbyte>().ToString();
                    break;
                case SecsFormat.I2:
                    value = item.FirstValue<short>().ToString();
                    break;
                case SecsFormat.I4:
                    value = item.FirstValue<int>().ToString();
                    break;
                case SecsFormat.I8:
                    value = item.FirstValue<long>().ToString();
                    break;
                case SecsFormat.U1:
                    value = item.FirstValue<byte>().ToString();
                    break;
                case SecsFormat.U2:
                    value = item.FirstValue<ushort>().ToString();
                    break;
                case SecsFormat.U4:
                    value = item.FirstValue<uint>().ToString();
                    break;
                case SecsFormat.U8:
                    value = item.FirstValue<ulong>().ToString();
                    break;
                case SecsFormat.F4:
                    value = item.FirstValue<float>().ToString();
                    break;
                case SecsFormat.F8:
                    value = item.FirstValue<double>().ToString();
                    break;
                case SecsFormat.Boolean:
                    value = item.FirstValue<bool>().ToString();
                    break;
                default:
                    value = string.Empty;
                    break;
            }
            return value;

        }

    }
}
