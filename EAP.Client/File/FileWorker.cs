using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Sfis;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Secs4Net;
using Service.Utils;
using System.Net;
using static Secs4Net.Item;
using System.IO;

namespace EAP.Client.File
{

    internal class FileWorker : BackgroundService
    {
        private readonly ILog traLog = LogManager.GetLogger("Trace");

        private readonly IConfiguration configuration;
        private readonly ISecsGem secsGem;
        private readonly ISecsConnection hsmsConnection;

        public FileWorker(IConfiguration configuration, ISecsGem secsGem, ISecsConnection hsmsConnection)
        {
            this.configuration = configuration;
            this.secsGem = secsGem;
            this.hsmsConnection = hsmsConnection;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (MainForm.Instance == null || hsmsConnection.State != ConnectionState.Selected)
            {
                await Task.Delay(500);
            }
            var machinePath = configuration.GetSection("Custom")["MachinePath"];
            var machineUserName = configuration.GetSection("Custom")["MachineUserName"];
            var machinePassword = configuration.GetSection("Custom")["MachinePassword"];

            if (!Directory.Exists(machinePath))
            {
                if (!string.IsNullOrEmpty(machineUserName))
                {
                    NetworkCredential networkCredential = new NetworkCredential(machineUserName, machinePassword);
                    try
                    {
                        NetworkConnection networkConnection = new NetworkConnection(machinePath, networkCredential);
                    }
                    catch (Exception ex)
                    {
                        traLog.Error(ex.ToString());
                        ConnectLan(machinePath, machineUserName, machinePassword);
                    }
                }
            }

            if (!Directory.Exists(machinePath))
            {
                traLog.Error($"设备文件夹路径无法访问，请检查设定后重启本程序:{machinePath}");
            }
            else
            {
                await UpdateLotActionStatus();

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(5000);
                    try
                    {
                        var sfisIp = configuration.GetSection("Custom")["SfisIp"];
                        var sfisPort = int.Parse(configuration.GetSection("Custom")["SfisPort"]);

                        var files = Directory.GetFiles(machinePath, "*.json", SearchOption.TopDirectoryOnly);
                        if (files.Length > 0)
                        {
                            var processedFolder = Path.Combine(machinePath, "Processed");
                            if (!Directory.Exists(processedFolder)) Directory.CreateDirectory(processedFolder);
                            var file = files.OrderBy(f => System.IO.File.GetLastWriteTime(f)).First();
                            traLog.Info($"检测到新的json文件，开始处理:{Path.GetFileName(file)}");


                            try
                            {
                                var content = System.IO.File.ReadAllText(file);
                                var fileData = JsonConvert.DeserializeObject<BoardDataEvent>(content);

                                var panelSn = fileData.msgBody.boardID;
                                var recipeName = fileData.msgBody.recipeName;
                                MainForm.Instance.UpdatePanelAndMachineRecipe(panelSn, recipeName);

                                await UpdateLotActionStatus();

                                var getModelProjextReq = $"EQXXXXXX01,{panelSn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_PROJECT_NAME_INFO=???";

                                BaymaxService baymax = new BaymaxService();
                                var trans = baymax.GetBaymaxTrans(sfisIp, sfisPort, getModelProjextReq).Result;
                                if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                                {
                                    Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                      .Where(keyValueArray => keyValueArray.Length == 2)
                      .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                                    //string modelName = sfisParameters["SN_MODEL_NAME_INFO"];//第一种
                                    string modelName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[0];
                                    string projectName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[1];

                                    MainForm.Instance.UpdateModelnameAndProjectName(modelName, projectName);

                                    if (MainForm.Instance.isAutoCheckRecipe)
                                    {
                                        var disableLotActionCommand = new SecsMessage(2, 15)
                                        {
                                            SecsItem = L(L(U4(22041), I4(0)))
                                        };

                                        if (recipeName.Substring(0, 9) == modelName.Substring(0, 9))//TODO: 等ME定义比较规则
                                        {

                                        }
                                        else
                                        {
                                            traLog.Error($"设备当前recipe:{recipeName}与Modelname：{modelName}不匹配，禁止进板！");
                                            await secsGem.SendAsync(disableLotActionCommand);
                                        }
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                traLog.Error($"处理文件{Path.GetFileName(file)}异常");
                                traLog.Error(ex.ToString());
                            }
                            //移动文件到Processed文件夹
                            var processedFile = Path.Combine(processedFolder, Path.GetFileName(file));
                            if(System.IO.File.Exists(processedFile)) System.IO.File.Delete(processedFile);
                            System.IO.File.Move(file, processedFile,true);
                   
                        }
                    }
                    catch (Exception ee)
                    {
                        traLog.Error(ee.ToString());
                        continue;
                    }
                }
            }
        }

        public string ConnectLan(string p_Path, string p_UserName, string p_PassWord)
        {
            System.Diagnostics.Process _Process = new System.Diagnostics.Process();
            _Process.StartInfo.FileName = "cmd.exe";
            _Process.StartInfo.UseShellExecute = false;
            _Process.StartInfo.RedirectStandardInput = true;
            _Process.StartInfo.RedirectStandardOutput = true;
            _Process.StartInfo.CreateNoWindow = true;
            _Process.Start();
            //NET USE //192.168.0.1 PASSWORD /USER:UserName
            var UPconfig = string.IsNullOrEmpty(p_UserName) ? "" : p_PassWord + " /user:" + p_UserName;
            _Process.StandardInput.WriteLine("net use " + p_Path + " " + UPconfig);
            _Process.StandardInput.WriteLine("exit");
            _Process.WaitForExit();
            string _ReturnText = _Process.StandardOutput.ReadToEnd();// 得到cmd.exe的输出 
            _Process.Close();
            return _ReturnText;
        }

        private async Task UpdateLotActionStatus()
        {
            var checkLotActionStatus = new SecsMessage(2, 13)
            {
                SecsItem = L(U4(22041))
            };
            var checkLotActionStatusResponse = await secsGem.SendAsync(checkLotActionStatus);
            var lotActionStatus = checkLotActionStatusResponse.SecsItem[0].FirstValue<int>() == 1;
            MainForm.Instance.UpdateLotActionStatus(lotActionStatus);
        }
    }
}
