using EAP.Client.Model.Database;
using EAP.Client.RabbitMq;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.LogFileWatcher
{
    class LogFileWatcherService : BackgroundService
    {
        internal static ILog dbgLog = LogManager.GetLogger("Debug");

        private System.Threading.Timer _logFileWatcherTimer;
        private readonly IConfiguration configuration;
        private readonly RabbitMqService rabbitMqService;
        private ISqlSugarClient sqlSugarClient;

        public LogFileWatcherService(IConfiguration configuration, RabbitMqService rabbitMqService, ISqlSugarClient sqlSugarClient)
        {
            this.configuration = configuration;
            this.rabbitMqService = rabbitMqService;
            this.sqlSugarClient = sqlSugarClient;

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = configuration.GetValue<int>("CycleTime");

            _logFileWatcherTimer = new System.Threading.Timer(delegate
            {
                try
                {
                    dbgLog.Info("CollectLogJob Start");

                    var configs = configuration.GetSection("MachineConfigs").Get<IEnumerable<MachineConfig>>();
                    var mainEquipmentId = configuration.GetSection("Custom")["EquipmentId"];
                    var index = 0;
                    foreach (var machineConfig in configs)
                    {
                        var equipmentId = mainEquipmentId + "_" + index;
                        var stateMaxTime = sqlSugarClient.Queryable<EquipmentState>().Where(x => x.EquipmentId == equipmentId).Max(x => x.EventTime);
                        var alarmMaxTime = sqlSugarClient.Queryable<EquipmentAlarm>().Where(x => x.EquipmentId == equipmentId).Max(x => x.EventTime);
                        var parseLogTime = stateMaxTime > alarmMaxTime ? stateMaxTime : alarmMaxTime;
                        //最多取前15天的数据
                        parseLogTime = parseLogTime > DateTime.Now.AddDays(-180) ? parseLogTime : DateTime.Now.AddDays(-180);
                        dbgLog.Info($"CollectLogJob from {parseLogTime} ");
                        var files = GetFiles(machineConfig, parseLogTime);
                        var totalStateCount = 0;
                        foreach (var file in files)
                        {
                            var data = ParseLog(equipmentId, file, machineConfig, parseLogTime);
                            totalStateCount += data.EquipmentStates.Count;
                            if (data.EquipmentStates.Count() > 0 || data.EquipmentAlarms.Count() > 0)
                            {
                                dbgLog.Info($"Collect Log:{file.FullName}, States Count:{data.EquipmentStates.Count()}, Alarms Count:{data.EquipmentAlarms.Count()}");
                                sqlSugarClient.Insertable(data.EquipmentStates).ExecuteCommand();
                                sqlSugarClient.Insertable(data.EquipmentAlarms).ExecuteCommand();
                            }
                        }

                        index++;
                    }
                }
                catch (Exception ex)
                {
                    dbgLog.Error(ex.ToString());
                }

                try
                {
                    dbgLog.Info("UploadDataJob Start");



                    var allstates = sqlSugarClient.Queryable<EquipmentState>().Where(x => x.Uploaded == false).OrderBy(it => it.EventTime).ToList();
                    var allalarms = sqlSugarClient.Queryable<EquipmentAlarm>().Where(x => x.Uploaded == false).ToList();

                    var eqids = allstates.Select(it => it.EquipmentId).Distinct().ToList();
                    var eqids1 = allalarms.Select(it => it.EquipmentId).Distinct().ToList();
                    eqids = eqids.Concat(eqids1).Select(it => it.Split('_')[0]).Distinct().ToList();

                    int noLogIdleTime = int.Parse(configuration["NoLogIdleTime"] ?? "600");

                    foreach (var eqid in eqids)
                    {

                        var states = allstates.Where(it => it.EquipmentId.StartsWith(eqid)).OrderBy(it => it.EventTime).ToList();
                        var alarms = allalarms.Where(it => it.EquipmentId.StartsWith(eqid)).OrderBy(it => it.EventTime).ToList();
                        var lastState = string.Empty;
                        var lastStateTime = DateTime.MaxValue;


                        dbgLog.Info($"{eqid} 共有{states.Count}条状态，{alarms.Count}条报警待上传");

                        foreach (var state in states)
                        {
                            //上次状态是Run，且中间没有别的状态超过了noLogIdleTime秒，插入一个Idle
                            if (lastState.ToUpper() == "RUN" && lastStateTime.AddSeconds(noLogIdleTime) < state.EventTime)
                            {
                                dbgLog.Info($"{state.EquipmentId} Run状态时间{lastStateTime}，超过{noLogIdleTime}秒没有新状态，插入一个Idle状态");
                                var para = new Dictionary<string, object> {
                                { "EQID",state.EquipmentId.Split('_')[0] },//截取下划线之前的EQID
                                { "DateTime",lastStateTime.AddSeconds(noLogIdleTime)},
                                { "EQType","Log" },
                                { "Status","Idle"}
                            };
                                RabbitMqTransaction trans = new RabbitMqTransaction
                                {
                                    TransactionName = "EquipmentStatus",
                                    Parameters = para
                                };
                                rabbitMqService.Produce("EAP.Services", trans);
                                lastState = "Idle";//更新上一个状态为Idle
                            }

                            if (state.State != lastState)//和上一个状态不一样才上传
                            {
                                var para = new Dictionary<string, object> {
                        { "EQID",state.EquipmentId.Split('_')[0] },//截取下划线之前的EQID
                        { "DateTime",state.EventTime},
                        { "EQType","Log" },
                        { "Status",state.State}
                        };
                                RabbitMqTransaction trans = new RabbitMqTransaction
                                {
                                    TransactionName = "EquipmentStatus",
                                    Parameters = para
                                };
                                rabbitMqService.Produce("EAP.Services", trans);
                            }
                            lastState = state.State;
                            lastStateTime = state.EventTime;
                            state.Uploaded = true;
                            sqlSugarClient.Updateable(state).ExecuteCommand();
                        }
                        foreach (var alarm in alarms)
                        {
                            var para = new Dictionary<string, object> {
                            { "AlarmEqp", alarm.EquipmentId.Split('_')[0]},
                            { "AlarmCode",alarm.AlarmCode},
                            { "AlarmText",alarm.AlarmText},
                            { "AlarmSource", "Log"},
                            { "AlarmTime",alarm.EventTime},
                            { "AlarmSet",true}
                        };
                            var trans = new RabbitMqTransaction
                            {
                                TransactionName = "EquipmentAlarm",
                                Parameters = para,
                            };
                            rabbitMqService.Produce("EAP.Services", trans);

                            alarm.Uploaded = true;
                            sqlSugarClient.Updateable(alarm).ExecuteCommand();
                        }

                    }
                    //三天内每个设备的最新状态时间小于当前时间-noLogIdleTime的，插入一个Idle
                    var fromDate = DateTime.Now.AddDays(-90);
                    var allEqidMaxTime = sqlSugarClient.Queryable<EquipmentState>().Where(x => x.EventTime > fromDate).ToList()
                        .GroupBy(it => it.EquipmentId.Split('_')[0]).
                        Select(it => new { EqId = it.Key, Time = it.Max(y => y.EventTime) }).ToList();
                    foreach (var item in allEqidMaxTime)
                    {
                        if (item.Time.AddSeconds(noLogIdleTime) < DateTime.Now)
                        {
                            dbgLog.Info($"{item.EqId} 最新状态时间为{item.Time},超过{noLogIdleTime}秒无状态，插入一个Idle");
                            var para = new Dictionary<string, object> {
                                { "EQID",item.EqId.Split('_')[0] },//截取下划线之前的EQID
                                { "DateTime",DateTime.Now},
                                { "EQType","Log" },
                                { "Status","Idle"}
                            };
                            RabbitMqTransaction trans = new RabbitMqTransaction
                            {
                                TransactionName = "EquipmentStatus",
                                Parameters = para
                            };
                            rabbitMqService.Produce("EAP.Services", trans);
                        }
                    }

                }
                catch (Exception ex)
                {
                    dbgLog.Error(ex.ToString());
                }

                //重置定时器
                _logFileWatcherTimer.Change(
               dueTime: TimeSpan.FromSeconds(interval), // 5秒后再次触发
               period: TimeSpan.FromMilliseconds(Timeout.Infinite)); // 保持单次触发模式

            }, null, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(Timeout.Infinite));

            return Task.CompletedTask;
        }

        Utils.NetworkConnection connection;
        private List<FileInfo> GetFiles(MachineConfig config, DateTime parseLogTime)
        {
            List<FileInfo> files = new List<FileInfo>();
            try
            {
                foreach (var path in config.FilePaths)
                {
                    //判断文件夹路径是否存在
                    if (!Directory.Exists(path))
                    {
                        if (!string.IsNullOrEmpty(config.PathUserName))
                        {
                            NetworkCredential networkCredential = new NetworkCredential(config.PathUserName, config.PathUserPassword);
                            try
                            {
                                connection = new Utils.NetworkConnection(path, networkCredential);
                            }
                            catch (Exception ex)
                            {
                                dbgLog.Error(ex.ToString());
                                ConnectLan(path, config.PathUserName, config.PathUserPassword);
                            }
                        }
                        if (!Directory.Exists(path))
                        {
                            dbgLog.Error($"文件夹路径不存在:{path}");
                            continue;
                        }
                    }
                    DirectoryInfo di = new DirectoryInfo(path);
                    foreach (var ext in config.FileExtensions)
                    {
                        //files.AddRange(di.GetFiles("*." + ext.TrimStart('.'), SearchOption.AllDirectories));
                        files.AddRange(GetFilesRecursive(di, ext, parseLogTime));
                    }

                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
            }

            return files;
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
        class EquipmentData
        {
            public List<EquipmentState> EquipmentStates { get; set; }
            public List<EquipmentAlarm> EquipmentAlarms { get; set; }
        }

        private EquipmentData ParseLog(string equipmentId, FileInfo file, MachineConfig config, DateTime parseLogTime)
        {
            List<EquipmentState> states = new List<EquipmentState>();
            List<EquipmentAlarm> alarms = new List<EquipmentAlarm>();
            try
            {
                using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.GetEncoding(config.Encoding ?? "UTF-8")))
                {
                    string line;
                    var currentTime = DateTime.MinValue;
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            line = sr.ReadLine();
                            var match = System.Text.RegularExpressions.Regex.Match(line, config.TimeRegex);
                            if (match.Success)
                            {

                                currentTime = DateTime.ParseExact(match.Value, config.TimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            if (currentTime <= parseLogTime)
                            {
                                continue;
                            }
                            //Equipment State
                            foreach (var state in config.StateDict)
                            {
                                if (line.Contains(state.Key))
                                {
                                    states.Add(new EquipmentState
                                    {
                                        EventTime = currentTime,
                                        EquipmentId = equipmentId,
                                        State = state.Value
                                    });
                                }
                            }
                            //Equipment Alarm
                            foreach (var alarm in config.AlarmDict)
                            {
                                if (line.Contains(alarm.Key))
                                {
                                    alarms.Add(new EquipmentAlarm
                                    {
                                        EventTime = currentTime,
                                        EquipmentId = equipmentId,
                                        AlarmCode = alarm.Value,
                                        AlarmText = line
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            dbgLog.Error(ex.ToString());
                            continue;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
            }
            return new EquipmentData
            {
                EquipmentStates = states,
                EquipmentAlarms = alarms
            };
        }

        private IEnumerable<FileInfo> GetFilesRecursive(DirectoryInfo directory, string searchPattern, DateTime parseLogTime)
        {
            var result = new List<FileInfo>();
            try
            {
                // 处理当前目录下的文件
                foreach (var file in directory.GetFiles(searchPattern))
                {
                    if (file.LastWriteTime > parseLogTime)
                    {
                        result.Add(file);
                    }
                }

                // 递归处理子目录
                foreach (var subDirectory in directory.GetDirectories())
                {
                    result.AddRange(GetFilesRecursive(subDirectory, searchPattern, parseLogTime));
                }
            }
            catch (UnauthorizedAccessException)
            {
                // 忽略权限不足的文件/目录
            }
            catch (DirectoryNotFoundException)
            {
                // 忽略不存在的目录
            }
            return result;
        }

    }
}
