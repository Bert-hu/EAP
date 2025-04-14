using EAP.Client.Forms;
using EAP.Client.Model;
using EAP.Client.RabbitMq;
using EAP.Client.Sfis;
using EAP.Client.Utils;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sunny.UI;
using System.Net;

namespace EAP.Client.File
{
    internal class FileSfisWorker : BackgroundService
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly ILog eqpLog = LogManager.GetLogger("Equipment");
        private readonly ILog traLog = LogManager.GetLogger("Trace");


        private IConfiguration configuration;
        private RabbitMqService rabbitMqService;

        private readonly System.Threading.Timer _heartBeatTimer;
        public FileSfisWorker(IConfiguration configuration, RabbitMqService rabbitMqService)
        {
            this.configuration = configuration;
            this.rabbitMqService = rabbitMqService;


        }
        NetworkConnection sfisConnection;
        NetworkConnection machineConnection;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (MainForm.Instance == null)
            {
                await Task.Delay(500);
            }


            var sfisPath = configuration.GetSection("Custom")["SfisPath"];
            var sfisUserName = configuration.GetSection("Custom")["SfisUserName"];
            var sfisPassword = configuration.GetSection("Custom")["SfisPassword"];
            var machinePath = configuration.GetSection("Custom")["MachinePath"];
            var machineRecipePath = configuration.GetSection("Custom")["MachineRecipePath"];
            var machineUserName = configuration.GetSection("Custom")["MachineUserName"];
            var machinePassword = configuration.GetSection("Custom")["MachinePassword"];

            //检查Sfis过站文件夹
            if (!Directory.Exists(sfisPath))
            {

                if (!string.IsNullOrEmpty(sfisUserName))
                {
                    NetworkCredential networkCredential = new NetworkCredential(sfisUserName, sfisPassword);
                    try
                    {
                        sfisConnection = new NetworkConnection(sfisPath, networkCredential);
                    }
                    catch (Exception ex)
                    {
                        dbgLog.Error(ex.ToString());
                        ConnectLan(sfisPath, sfisUserName, sfisPassword);
                    }
                }

            }

            //检查设备过站文件夹
            if (!Directory.Exists(machinePath))
            {
                if (!string.IsNullOrEmpty(machineUserName))
                {
                    NetworkCredential networkCredential = new NetworkCredential(machineUserName, machinePassword);
                    try
                    {
                        machineConnection = new NetworkConnection(machinePath, networkCredential);
                    }
                    catch (Exception ex)
                    {
                        dbgLog.Error(ex.ToString());
                        ConnectLan(machinePath, machineUserName, machinePassword);
                    }
                }
            }

            //检查设备Recipe文件夹
            if (!Directory.Exists(machineRecipePath))
            {
                if (!string.IsNullOrEmpty(machineUserName))
                {
                    NetworkCredential networkCredential = new NetworkCredential(machineUserName, machinePassword);
                    try
                    {
                        machineConnection = new NetworkConnection(machineRecipePath, networkCredential);
                    }
                    catch (Exception ex)
                    {
                        dbgLog.Error(ex.ToString());
                        ConnectLan(machineRecipePath, machineUserName, machinePassword);
                    }
                }
            }


            if (!Directory.Exists(machinePath))
            {
                traLog.Error($"设备过站文件夹路径无法访问，请检查设定后重启本程序:{machinePath}");
            }

            else if (!Directory.Exists(sfisPath))
            {
                traLog.Error($"SFIS过站文件夹路径无法访问，请检查设定后重启本程序:{sfisPath}");
            }
            else if (!Directory.Exists(machineRecipePath))
            {
                traLog.Error($"设备Recipe文件夹路径无法访问，请检查设定后重启本程序:{machineRecipePath}");
            }
            else
            {
                var currentRecipeName = string.Empty;
                var currentRecipeFileName = Directory.GetFiles(machineRecipePath, "*.rcp", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (!string.IsNullOrEmpty(currentRecipeFileName))
                {
                    currentRecipeName = Path.GetFileName(currentRecipeFileName);
                }
                else
                {
                    currentRecipeName = string.Empty;
                }
                MainForm.Instance.UpdateMachineRecipe(currentRecipeName);


                //每10s检查一次Sfis文件夹是否有.ttt文件
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var sfisIp = configuration.GetSection("Custom")["SfisIp"];
                        var sfisPort = int.Parse(configuration.GetSection("Custom")["SfisPort"]);

                        var rmsUrl = configuration.GetSection("Custom")["RmsApiUrl"];

                        var files = Directory.GetFiles(sfisPath, "*.ttt", SearchOption.TopDirectoryOnly);
                        if (files.Length > 0)
                        {
                            var errorFolder = Path.Combine(sfisPath, "Error");
                            if (!Directory.Exists(errorFolder))
                            {
                                Directory.CreateDirectory(errorFolder);
                            }

                            var file = files.OrderBy(f => System.IO.File.GetLastWriteTime(f)).First();
                            traLog.Info($"检测到Sfis文件夹有.ttt文件，开始处理:{Path.GetFileName(file)}");

                            var fileName = Path.GetFileNameWithoutExtension(file);
                            var panelId = fileName.Split('_')[3];

                            var getModelProjextReq = $"SMD_QUERY01,{panelId},7,M000001,JQ21-4FAP-99,,OK,PROJECT_NAME=??? MODEL_NAME=??? GROUP_NAME=???,,,,,,,,";

                            BaymaxService baymax = new BaymaxService();
                            var trans = baymax.GetBaymaxTrans(sfisIp, sfisPort, getModelProjextReq).Result;
                            traLog.Info(trans.BaymaxResponse);
                            if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                            {
                                Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
              .Where(keyValueArray => keyValueArray.Length == 2)
              .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                                string projectName = sfisParameters["PROJECT_NAME"];
                                string modelName = sfisParameters["MODEL_NAME"];
                                string groupName = sfisParameters["GROUP_NAME"];

                                MainForm.Instance.UpdatePanelInfo(panelId, modelName, projectName);


                                var getRecipeRes = RmsFunction.GetRecipeName(configuration, projectName);
                                if (getRecipeRes.Result)
                                {
                                    var recipeName = getRecipeRes.RecipeName;

                                    currentRecipeFileName = Directory.GetFiles(machineRecipePath, "*.rcp", SearchOption.TopDirectoryOnly).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(currentRecipeFileName))
                                    {
                                        currentRecipeName = Path.GetFileName(currentRecipeFileName);
                                    }
                                    else
                                    {
                                        currentRecipeName = string.Empty;
                                    }
                                    MainForm.Instance.UpdateMachineRecipe(currentRecipeName);

                                    if (currentRecipeName == recipeName)
                                    {
                                        var compareBodyRes = RmsFunction.CompareRecipeBody(rabbitMqService, configuration, recipeName);//这里会调用RabbitMq CompareRecipe
                                        traLog.Info(compareBodyRes.Message);
                                        if (compareBodyRes.Result)
                                        {
                                            //把文件移动到设备文件夹
                                            var machineFile = Path.Combine(machinePath, Path.GetFileName(file));
                                            if (System.IO.File.Exists(machineFile))
                                            {
                                                //重命名
                                                var machineFileNew = machineFile + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                                                System.IO.File.Move(machineFile, machineFileNew);
                                            }
                                            System.IO.File.Move(file, machineFile);
                                            //TODO: add traceability
                                            Traceability traceability = new Traceability();
                                            traceability.ProjectName = projectName;
                                            traceability.ModelName = modelName;
                                            traceability.EquipmentId = configuration.GetSection("Custom")["EquipmentId"];
                                            traceability.PanelSn = panelId;
                                            traceability.RecipeName = recipeName;
                                            traceability.InputTime = DateTime.Now;
                                            traceability.RecipePara = GetUnformattedRecipe.lastReadRecipePara; //调用RabbitMq CompareRecipe后这里一定不为空
                                                                                                               
                                        }

                                    }
                                    else
                                    {
                                        var errmsg = $"设备Recipe名称不匹配，SN:{panelId},RecipeName:{recipeName},MachineRecipeName:{currentRecipeFileName}";
                                        traLog.Error(errmsg);
                                        MainForm.Instance.ShowErrorDialog2(errmsg);

                                        //把文件移动到当前路径Error文件夹
                                        var errorFile = Path.Combine(errorFolder, Path.GetFileName(file)+ "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                                        System.IO.File.Move(file, errorFile);
                                    }
                                }
                                else
                                {
                                    var errmsg = $"获取设备Recipe名称失败，SN:{panelId},Error:{getRecipeRes.Message}";
                                    MainForm.Instance.ShowErrorDialog2(errmsg);
                                    traLog.Error(errmsg);
                                    //var errorFile = Path.Combine(errorFolder, Path.GetFileName(file) + "_" + DateTime.Now.ToString//("yyyyMMddHHmmss"));
                                    //System.IO.File.Move(file, errorFile);
                                }

                            }

                        }


                    }
                    catch (Exception ex)
                    {
                        traLog.Error(ex.Message, ex);
                    }
                    await Task.Delay(10000, stoppingToken);
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


    }
}
