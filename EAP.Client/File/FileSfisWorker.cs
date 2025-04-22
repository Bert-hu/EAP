using EAP.Client.Forms;
using EAP.Client.Model;
using EAP.Client.RabbitMq;
using EAP.Client.Sfis;
using EAP.Client.Utils;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
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

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    try
        //    {
        //        do
        //        {
        //            await Task.Delay(2000);
        //        }
        //        while (MainForm.Instance == null);

        //        var sfisPath = configuration.GetSection("Custom")["SfisPath"];
        //        var sfisUserName = configuration.GetSection("Custom")["SfisUserName"];
        //        var sfisPassword = configuration.GetSection("Custom")["SfisPassword"];
        //        var machinePath = configuration.GetSection("Custom")["MachinePath"];
        //        var machineRecipePath = configuration.GetSection("Custom")["MachineRecipePath"];
        //        var machineUserName = configuration.GetSection("Custom")["MachineUserName"];
        //        var machinePassword = configuration.GetSection("Custom")["MachinePassword"];
        //        var tracePath = configuration.GetSection("Custom")["TracePath"];
        //        var traceUserName = configuration.GetSection("Custom")["TraceUserName"];
        //        var tracePassword = configuration.GetSection("Custom")["TracePassword"];

        //        //检查Sfis过站文件夹
        //        if (!Directory.Exists(sfisPath))
        //        {

        //            if (!string.IsNullOrEmpty(sfisUserName))
        //            {
        //                NetworkCredential networkCredential = new NetworkCredential(sfisUserName, sfisPassword);
        //                try
        //                {
        //                    sfisConnection = new NetworkConnection(sfisPath, networkCredential);
        //                }
        //                catch (Exception ex)
        //                {
        //                    dbgLog.Error(ex.ToString());
        //                    ConnectLan(sfisPath, sfisUserName, sfisPassword);
        //                }
        //            }

        //        }

        //        //检查设备过站文件夹
        //        if (!Directory.Exists(machinePath))
        //        {
        //            if (!string.IsNullOrEmpty(machineUserName))
        //            {
        //                NetworkCredential networkCredential = new NetworkCredential(machineUserName, machinePassword);
        //                try
        //                {
        //                    machineConnection = new NetworkConnection(machinePath, networkCredential);
        //                }
        //                catch (Exception ex)
        //                {
        //                    dbgLog.Error(ex.ToString());
        //                    ConnectLan(machinePath, machineUserName, machinePassword);
        //                }
        //            }
        //        }

        //        //检查设备Recipe文件夹
        //        if (!Directory.Exists(machineRecipePath))
        //        {
        //            if (!string.IsNullOrEmpty(machineUserName))
        //            {
        //                NetworkCredential networkCredential = new NetworkCredential(machineUserName, machinePassword);
        //                try
        //                {
        //                    machineConnection = new NetworkConnection(machineRecipePath, networkCredential);
        //                }
        //                catch (Exception ex)
        //                {
        //                    dbgLog.Error(ex.ToString());
        //                    ConnectLan(machineRecipePath, machineUserName, machinePassword);
        //                }
        //            }
        //        }

        //        //检查Trace文件夹
        //        if (!Directory.Exists(tracePath))
        //        {
        //            if (!string.IsNullOrEmpty(traceUserName))
        //            {
        //                NetworkCredential networkCredential = new NetworkCredential(traceUserName, tracePassword);
        //                try
        //                {
        //                    machineConnection = new NetworkConnection(tracePath, networkCredential);
        //                }
        //                catch (Exception ex)
        //                {
        //                    dbgLog.Error(ex.ToString());
        //                    ConnectLan(tracePath, traceUserName, tracePassword);
        //                }
        //            }
        //        }


        //        if (!Directory.Exists(machinePath))
        //        {
        //            traLog.Error($"设备过站文件夹路径无法访问，请检查设定后重启本程序:{machinePath}");
        //        }

        //        else if (!Directory.Exists(sfisPath))
        //        {
        //            traLog.Error($"SFIS过站文件夹路径无法访问，请检查设定后重启本程序:{sfisPath}");
        //        }
        //        else if (!Directory.Exists(machineRecipePath))
        //        {
        //            traLog.Error($"设备Recipe文件夹路径无法访问，请检查设定后重启本程序:{machineRecipePath}");
        //        }
        //        else if (!Directory.Exists(tracePath))
        //        {
        //            traLog.Error($"Trace文件夹路径无法访问，请检查设定后重启本程序:{tracePath}");
        //        }
        //        else
        //        {
        //            var currentRecipeName = string.Empty;
        //            var currentRecipeFileFullName = Directory.GetFiles(machineRecipePath, "*.rcp", SearchOption.TopDirectoryOnly).FirstOrDefault();
        //            string currentRecipeFileName = string.Empty;
        //            if (!string.IsNullOrEmpty(currentRecipeFileFullName))
        //            {
        //                currentRecipeName = Path.GetFileName(currentRecipeFileFullName);
        //            }
        //            else
        //            {
        //                currentRecipeName = string.Empty;
        //            }
        //            MainForm.Instance.UpdateMachineRecipe(currentRecipeName);


        //            //每10s检查一次Sfis文件夹是否有.ttt文件
        //            while (!stoppingToken.IsCancellationRequested)
        //            {
        //                try
        //                {
        //                    var sfisIp = configuration.GetSection("Custom")["SfisIp"];
        //                    var sfisPort = int.Parse(configuration.GetSection("Custom")["SfisPort"]);

        //                    var rmsUrl = configuration.GetSection("Custom")["RmsApiUrl"];

        //                    var files = Directory.GetFiles(sfisPath, "*.ttt", SearchOption.TopDirectoryOnly);
        //                    if (files.Length > 0)
        //                    {
        //                        var errorFolder = Path.Combine(sfisPath, "Error");
        //                        if (!Directory.Exists(errorFolder))
        //                        {
        //                            Directory.CreateDirectory(errorFolder);
        //                        }

        //                        var file = files.OrderBy(f => System.IO.File.GetLastWriteTime(f)).First();
        //                        traLog.Info($"检测到Sfis文件夹有.ttt文件，开始处理:{Path.GetFileName(file)}");

        //                        var fileName = Path.GetFileNameWithoutExtension(file);
        //                        var panelId = fileName.Split('_')[3];

        //                        if (MainForm.Instance.isAutoCheckRecipe)//检查Recipe Name和Body
        //                        {
        //                            var getModelProjextReq = $"SMD_QUERY01,{panelId},7,M000001,JQ21-4FAP-99,,OK,PROJECT_NAME=??? MODEL_NAME=??? GROUP_NAME=???,,,,,,,,";

        //                            BaymaxService baymax = new BaymaxService();
        //                            var trans = baymax.GetBaymaxTrans(sfisIp, sfisPort, getModelProjextReq).Result;
        //                            traLog.Info(trans.BaymaxResponse);
        //                            if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
        //                            {
        //                                Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
        //              .Where(keyValueArray => keyValueArray.Length == 2)
        //              .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
        //                                string projectName = sfisParameters["PROJECT_NAME"];
        //                                string modelName = sfisParameters["MODEL_NAME"];
        //                                string groupName = sfisParameters["GROUP_NAME"];

        //                                MainForm.Instance.UpdatePanelInfo(panelId, modelName, projectName);


        //                                var getRecipeRes = RmsFunction.GetRecipeName(configuration, projectName);
        //                                if (getRecipeRes.Result)
        //                                {
        //                                    var recipeName = getRecipeRes.RecipeName;

        //                                    currentRecipeFileName = Directory.GetFiles(machineRecipePath, "*.rcp", SearchOption.TopDirectoryOnly).FirstOrDefault();
        //                                    if (!string.IsNullOrEmpty(currentRecipeFileName))
        //                                    {
        //                                        currentRecipeName = Path.GetFileName(currentRecipeFileName);
        //                                    }
        //                                    else
        //                                    {
        //                                        currentRecipeName = string.Empty;
        //                                    }
        //                                    MainForm.Instance.UpdateMachineRecipe(currentRecipeName);

        //                                    if (currentRecipeName == recipeName)
        //                                    {
        //                                        var compareBodyRes = RmsFunction.CompareRecipeBody(rabbitMqService, configuration, recipeName);//这里会调用RabbitMq CompareRecipe
        //                                        traLog.Info(compareBodyRes.Message);
        //                                        if (compareBodyRes.Result)
        //                                        {
        //                                            //把文件移动到设备文件夹
        //                                            var machineFile = Path.Combine(machinePath, Path.GetFileName(file));
        //                                            if (System.IO.File.Exists(machineFile))
        //                                            {
        //                                                //重命名
        //                                                var machineFileNew = machineFile + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
        //                                                System.IO.File.Move(machineFile, machineFileNew);
        //                                            }
        //                                            System.IO.File.Move(file, machineFile);

        //                                            Traceability traceability = new Traceability();
        //                                            traceability.ProjectName = projectName;
        //                                            traceability.ModelName = modelName;
        //                                            traceability.EquipmentId = configuration.GetSection("Custom")["EquipmentId"];
        //                                            traceability.PanelSn = panelId;
        //                                            traceability.RecipeName = recipeName;
        //                                            traceability.InputTime = DateTime.Now;
        //                                            traceability.RecipePara = GetUnformattedRecipe.lastReadRecipePara; //调用RabbitMq CompareRecipe后这里一定不为空
        //                                            var EquipmentId = configuration.GetSection("Custom")["EquipmentId"];

        //                                            var traceFile = $"{tracePath}\\{EquipmentId}\\{DateTime.Today.ToString("yyyyMMdd")}\\{panelId}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
        //                                            var traceFilePath = Path.GetDirectoryName(traceFile);
        //                                            if (!Directory.Exists(traceFilePath))
        //                                            {
        //                                                Directory.CreateDirectory(traceFilePath);
        //                                            }
        //                                            System.IO.File.WriteAllText(traceFile, JsonConvert.SerializeObject(traceability, Formatting.Indented));
        //                                            traLog.Info($"生成Trace文件到{traceFile}");
        //                                        }

        //                                    }
        //                                    else
        //                                    {
        //                                        var errmsg = $"设备Recipe名称不匹配，SN:{panelId},RecipeName:{recipeName},MachineRecipeName:{currentRecipeFileName}";
        //                                        traLog.Error(errmsg);
        //                                        MainForm.Instance.ShowErrorDialog2(errmsg);

        //                                        //把文件移动到当前路径Error文件夹
        //                                        var errorFile = Path.Combine(errorFolder, Path.GetFileName(file) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
        //                                        System.IO.File.Move(file, errorFile);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    var errmsg = $"获取设备Recipe名称失败，SN:{panelId},Error:{getRecipeRes.Message}";
        //                                    MainForm.Instance.ShowErrorDialog2(errmsg);
        //                                    traLog.Error(errmsg);
        //                                    //var errorFile = Path.Combine(errorFolder, Path.GetFileName(file) + "_" + DateTime.Now.ToString//("yyyyMMddHHmmss"));
        //                                    //System.IO.File.Move(file, errorFile);
        //                                }

        //                            }
        //                        }
        //                        else//不检查，只生成Trace
        //                        {
        //                            //移动
        //                            var machineFile = Path.Combine(machinePath, Path.GetFileName(file));
        //                            if (System.IO.File.Exists(machineFile))
        //                            {
        //                                //重命名
        //                                var machineFileNew = machineFile + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
        //                                System.IO.File.Move(machineFile, machineFileNew);
        //                            }
        //                            System.IO.File.Move(file, machineFile);


        //                            var machineRecipeText = System.IO.File.ReadAllText(currentRecipeFileFullName);
        //                            var machineRecipePara = GetUnformattedRecipe.GetRecipePara(currentRecipeFileFullName, machineRecipeText);
        //                            Traceability traceability = new Traceability();
        //                            traceability.EquipmentId = configuration.GetSection("Custom")["EquipmentId"];
        //                            traceability.PanelSn = panelId;
        //                            traceability.RecipeName = currentRecipeFileName;
        //                            traceability.InputTime = DateTime.Now;
        //                            traceability.RecipePara = GetUnformattedRecipe.lastReadRecipePara; //调用RabbitMq CompareRecipe后这里一定不为空
        //                            var EquipmentId = configuration.GetSection("Custom")["EquipmentId"];

        //                            var traceFile = $"{tracePath}\\{EquipmentId}\\{DateTime.Today.ToString("yyyyMMdd")}\\{panelId}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
        //                            var traceFilePath = Path.GetDirectoryName(traceFile);
        //                            if (!Directory.Exists(traceFilePath))
        //                            {
        //                                Directory.CreateDirectory(traceFilePath);
        //                            }
        //                            System.IO.File.WriteAllText(traceFile, JsonConvert.SerializeObject(traceability, Formatting.Indented));
        //                            traLog.Info($"生成Trace文件到{traceFile}");
        //                        }


        //                    }


        //                }
        //                catch (Exception ex)
        //                {
        //                    traLog.Error(ex.Message, ex);
        //                }
        //                await Task.Delay(10000, stoppingToken);
        //            }
        //        }
        //    }
        //    catch (Exception ee)
        //    {
        //        traLog.Error(ee.ToString());
        //    }
        //}
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                do
                {
                    await Task.Delay(2000);
                }
                while (MainForm.Instance == null);

                var config = GetConfiguration();

                CheckAndConnectFolder(config.SfisPath, config.SfisUserName, config.SfisPassword);
                CheckAndConnectFolder(config.MachinePath, config.MachineUserName, config.MachinePassword);
                CheckAndConnectFolder(config.MachineRecipePath, config.MachineUserName, config.MachinePassword);
                CheckAndConnectFolder(config.TracePath, config.TraceUserName, config.TracePassword);

                if (!Directory.Exists(config.MachinePath))
                {
                    traLog.Error($"设备过站文件夹路径无法访问，请检查设定后重启本程序:{config.MachinePath}");
                }
                else if (!Directory.Exists(config.SfisPath))
                {
                    traLog.Error($"SFIS过站文件夹路径无法访问，请检查设定后重启本程序:{config.SfisPath}");
                }
                else if (!Directory.Exists(config.MachineRecipePath))
                {
                    traLog.Error($"设备Recipe文件夹路径无法访问，请检查设定后重启本程序:{config.MachineRecipePath}");
                }
                else if (!Directory.Exists(config.TracePath))
                {
                    traLog.Error($"Trace文件夹路径无法访问，请检查设定后重启本程序:{config.TracePath}");
                }
                else
                {
                    var currentRecipeName = GetCurrentRecipeName(config.MachineRecipePath);
                    MainForm.Instance.UpdateMachineRecipe(currentRecipeName);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var sfisIp = config.SfisIp;
                            var sfisPort = config.SfisPort;
                            var rmsUrl = config.RmsApiUrl;

                            var files = Directory.GetFiles(config.SfisPath, "*.ttt", SearchOption.TopDirectoryOnly);
                            if (files.Length > 0)
                            {
                                var errorFolder = Path.Combine(config.SfisPath, "Error");
                                if (!Directory.Exists(errorFolder))
                                {
                                    Directory.CreateDirectory(errorFolder);
                                }

                                var file = files.OrderBy(f => System.IO.File.GetLastWriteTime(f)).First();
                                traLog.Info($"检测到Sfis文件夹有.ttt文件，开始处理:{Path.GetFileName(file)}");

                                var fileName = Path.GetFileNameWithoutExtension(file);
                                var panelId = fileName.Split('_')[3];

                                if (MainForm.Instance.isAutoCheckRecipe)
                                {
                                    await ProcessWithRecipeCheck(config, file, panelId, sfisIp, sfisPort, rmsUrl, errorFolder);
                                }
                                else
                                {
                                    ProcessWithoutRecipeCheck(config, file, panelId, currentRecipeName);
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
            catch (Exception ee)
            {
                traLog.Error(ee.ToString());
            }
        }

        private Configuration GetConfiguration()
        {
            return new Configuration
            {
                SfisPath = configuration.GetSection("Custom")["SfisPath"],
                SfisUserName = configuration.GetSection("Custom")["SfisUserName"],
                SfisPassword = configuration.GetSection("Custom")["SfisPassword"],
                MachinePath = configuration.GetSection("Custom")["MachinePath"],
                MachineRecipePath = configuration.GetSection("Custom")["MachineRecipePath"],
                MachineUserName = configuration.GetSection("Custom")["MachineUserName"],
                MachinePassword = configuration.GetSection("Custom")["MachinePassword"],
                TracePath = configuration.GetSection("Custom")["TracePath"],
                TraceUserName = configuration.GetSection("Custom")["TraceUserName"],
                TracePassword = configuration.GetSection("Custom")["TracePassword"],
                SfisIp = configuration.GetSection("Custom")["SfisIp"],
                SfisPort = int.Parse(configuration.GetSection("Custom")["SfisPort"]),
                RmsApiUrl = configuration.GetSection("Custom")["RmsApiUrl"],
                EquipmentId = configuration.GetSection("Custom")["EquipmentId"]
            };
        }

        private void CheckAndConnectFolder(string path, string username, string password)
        {
            if (!Directory.Exists(path))
            {
                if (!string.IsNullOrEmpty(username))
                {
                    NetworkCredential networkCredential = new NetworkCredential(username, password);
                    try
                    {
                        var connection = new NetworkConnection(path, networkCredential);
                    }
                    catch (Exception ex)
                    {
                        dbgLog.Error(ex.ToString());
                        ConnectLan(path, username, password);
                    }
                }
            }
        }

        private string GetCurrentRecipeName(string machineRecipePath)
        {
            var currentRecipeFileFullName = Directory.GetFiles(machineRecipePath, "*.rcp", SearchOption.TopDirectoryOnly).FirstOrDefault();
            return !string.IsNullOrEmpty(currentRecipeFileFullName) ? Path.GetFileName(currentRecipeFileFullName) : string.Empty;
        }

        private async Task ProcessWithRecipeCheck(Configuration config, string file, string panelId, string sfisIp, int sfisPort, string rmsUrl, string errorFolder)
        {
            var getModelProjextReq = $"SMD_QUERY01,{panelId},7,M000001,JQ21-4FAP-99,,OK,PROJECT_NAME=??? MODEL_NAME=??? GROUP_NAME=???,,,,,,,,";
            BaymaxService baymax = new BaymaxService();
            var trans = await baymax.GetBaymaxTrans(sfisIp, sfisPort, getModelProjextReq);
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

                    var currentRecipeName = GetCurrentRecipeName(config.MachineRecipePath);
                    MainForm.Instance.UpdateMachineRecipe(currentRecipeName);

                    if (currentRecipeName == recipeName)
                    {
                        var compareBodyRes = RmsFunction.CompareRecipeBody(rabbitMqService, configuration, recipeName);
                        traLog.Info(compareBodyRes.Message);
                        if (compareBodyRes.Result)
                        {
                            MoveFileToMachineFolder(config.MachinePath, file);
                            GenerateTraceFile(config, panelId, recipeName, modelName, projectName);
                        }
                    }
                    else
                    {
                        var errmsg = $"设备Recipe名称不匹配，SN:{panelId},RecipeName:{recipeName},MachineRecipeName:{currentRecipeName}";
                        traLog.Error(errmsg);
                        MainForm.Instance.ShowErrorDialog2(errmsg);
                        MoveFileToErrorFolder(errorFolder, file);
                    }
                }
                else
                {
                    var errmsg = $"获取设备Recipe名称失败，SN:{panelId},Error:{getRecipeRes.Message}";
                    MainForm.Instance.ShowErrorDialog2(errmsg);
                    traLog.Error(errmsg);
                }
            }
        }

        private void ProcessWithoutRecipeCheck(Configuration config, string file, string panelId, string currentRecipeName)
        {
            MoveFileToMachineFolder(config.MachinePath, file);
            var recipeFullPath = Directory.GetFiles(config.MachineRecipePath, currentRecipeName, SearchOption.TopDirectoryOnly).FirstOrDefault();
            var machineRecipeText = System.IO.File.ReadAllText(recipeFullPath);
            var machineRecipePara = GetUnformattedRecipe.GetRecipePara(currentRecipeName, machineRecipeText);
            GenerateTraceFile(config, panelId, currentRecipeName);
        }

        private void MoveFileToMachineFolder(string machinePath, string file)
        {
            var machineFile = Path.Combine(machinePath, Path.GetFileName(file));
            if (System.IO.File.Exists(machineFile))
            {
                var machineFileNew = machineFile + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                System.IO.File.Move(machineFile, machineFileNew);
            }
            System.IO.File.Move(file, machineFile);
        }

        private void MoveFileToErrorFolder(string errorFolder, string file)
        {
            var errorFile = Path.Combine(errorFolder, Path.GetFileName(file) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            System.IO.File.Move(file, errorFile);
        }

        private void GenerateTraceFile(Configuration config, string panelId, string recipeName, string modelName = "", string projectName = "")
        {
            Traceability traceability = new Traceability();
            traceability.EquipmentId = config.EquipmentId;
            traceability.PanelSn = panelId;
            traceability.RecipeName = recipeName;
            traceability.InputTime = DateTime.Now;
            traceability.ModelName = modelName;
            traceability.ProjectName = projectName;
            traceability.RecipePara = GetUnformattedRecipe.lastReadRecipePara;


            var traceFile = $"{config.TracePath}\\{config.EquipmentId}\\{DateTime.Today.ToString("yyyyMMdd")}\\{panelId}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
            var traceFilePath = Path.GetDirectoryName(traceFile);
            if (!Directory.Exists(traceFilePath))
            {
                Directory.CreateDirectory(traceFilePath);
            }
            System.IO.File.WriteAllText(traceFile, JsonConvert.SerializeObject(traceability, Formatting.Indented));
            traLog.Info($"生成Trace文件到{traceFile}");
        }

        private class Configuration
        {
            public string SfisPath { get; set; }
            public string SfisUserName { get; set; }
            public string SfisPassword { get; set; }
            public string MachinePath { get; set; }
            public string MachineRecipePath { get; set; }
            public string MachineUserName { get; set; }
            public string MachinePassword { get; set; }
            public string TracePath { get; set; }
            public string TraceUserName { get; set; }
            public string TracePassword { get; set; }
            public string SfisIp { get; set; }
            public int SfisPort { get; set; }
            public string RmsApiUrl { get; set; }
            public string EquipmentId { get; set; }
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
