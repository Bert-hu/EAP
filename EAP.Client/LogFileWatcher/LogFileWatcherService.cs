using EAP.Client.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.LogFileWatcher
{
    class LogFileWatcherService : BackgroundService
    {
        private System.Threading.Timer _equipmentStatusTimer;
        private readonly IConfiguration configuration;
        private readonly RabbitMqService rabbitMqService;

        public LogFileWatcherService(IConfiguration configuration, RabbitMqService rabbitMqService)
        {
            this.configuration = configuration;
            this.rabbitMqService = rabbitMqService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = configuration.GetValue<int>("CycleTime");



            _equipmentStatusTimer = new System.Threading.Timer(delegate
            {
                try
                {
                    log.Info("CollectLogJob Start");
                    var db = DbFactory.GetSqliteSugarClient();

                    var config = LogConfig.GetConfig();
                    foreach (var machineConfig in config.MachineConfigs)
                    {
                        var stateMaxTime = db.Queryable<EquipmentState>().Where(x => x.EquipmentId == machineConfig.EquipmentId).Max(x => x.EventTime);
                        var alarmMaxTime = db.Queryable<EquipmentAlarm>().Where(x => x.EquipmentId == machineConfig.EquipmentId).Max(x => x.EventTime);
                        var parseLogTime = stateMaxTime > alarmMaxTime ? stateMaxTime : alarmMaxTime;
                        //最多取前15天的数据
                        parseLogTime = parseLogTime > DateTime.Now.AddDays(-90) ? parseLogTime : DateTime.Now.AddDays(-90);
                        log.Info($"CollectLogJob from {parseLogTime} ");
                        var files = GetFiles(machineConfig, parseLogTime);
                        var totalStateCount = 0;
                        foreach (var file in files)
                        {
                            var data = ParseLog(file, machineConfig, parseLogTime);
                            totalStateCount += data.EquipmentStates.Count;
                            if (data.EquipmentStates.Count() > 0 || data.EquipmentAlarms.Count() > 0)
                            {
                                log.Info($"Collect Log:{file.FullName}, States Count:{data.EquipmentStates.Count()}, Alarms Count:{data.EquipmentAlarms.Count()}");
                                db.Insertable(data.EquipmentStates).ExecuteCommand();
                                db.Insertable(data.EquipmentAlarms).ExecuteCommand();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }

                try
                {
                    log.Info("UploadDataJob Start");
                    var db = DbFactory.GetSqliteSugarClient();
                    var config = LogConfig.GetConfig();
                    if (string.IsNullOrEmpty(config.RabbitMqHost)) return;
                    ConnectionFactory factory = new ConnectionFactory();
                    factory = new ConnectionFactory();
                    factory.HostName = config.RabbitMqHost;
                    factory.Port = config.RabbitMqPort;
                    factory.UserName = config.RabbitMqUserName;
                    factory.Password = config.RabbitMqPassword;
                    var connection = factory.CreateConnection();
                    var channel = connection.CreateModel();
                    Dictionary<string, object> arguments = new Dictionary<string, object>() { { "x-message-ttl", 300000 } };
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;
                    properties.Expiration = (30 * 1000).ToString();


                    var allstates = db.Queryable<EquipmentState>().Where(x => x.Uploaded == false).OrderBy(it => it.EventTime).ToList();
                    var allalarms = db.Queryable<EquipmentAlarm>().Where(x => x.Uploaded == false).ToList();

                    var eqids = allstates.Select(it => it.EquipmentId).Distinct().ToList();
                    var eqids1 = allalarms.Select(it => it.EquipmentId).Distinct().ToList();
                    eqids = eqids.Concat(eqids1).Select(it => it.Split('_')[0]).Distinct().ToList();

                    int noLogIdleTime = config.NoLogIdleTime ?? 600;

                    foreach (var eqid in eqids)
                    {

                        var states = allstates.Where(it => it.EquipmentId.StartsWith(eqid)).OrderBy(it => it.EventTime).ToList();
                        var alarms = allalarms.Where(it => it.EquipmentId.StartsWith(eqid)).OrderBy(it => it.EventTime).ToList();
                        var lastState = string.Empty;
                        var lastStateTime = DateTime.MaxValue;


                        log.Info($"{eqid} 共有{states.Count}条状态，{alarms.Count}条报警待上传");

                        foreach (var state in states)
                        {
                            //上次状态是Run，且中间没有别的状态超过了noLogIdleTime秒，插入一个Idle
                            if (lastState.ToUpper() == "RUN" && lastStateTime.AddSeconds(noLogIdleTime) < state.EventTime)
                            {
                                log.Info($"{state.EquipmentId} Run状态时间{lastStateTime}，超过{noLogIdleTime}秒没有新状态，插入一个Idle状态");
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
                                channel.BasicPublish("", "EAP.Services", properties, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(trans)));
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
                                channel.BasicPublish("", "EAP.Services", properties, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(trans)));
                            }
                            lastState = state.State;
                            lastStateTime = state.EventTime;
                            state.Uploaded = true;
                            db.Updateable(state).ExecuteCommand();
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
                            channel.BasicPublish("", "EAP.Services", properties, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(trans)));

                            alarm.Uploaded = true;
                            db.Updateable(alarm).ExecuteCommand();
                        }

                    }
                    //三天内每个设备的最新状态时间小于当前时间-noLogIdleTime的，插入一个Idle
                    var fromDate = DateTime.Now.AddDays(-90);
                    var allEqidMaxTime = db.Queryable<EquipmentState>().Where(x => x.EventTime > fromDate).ToList()
                        .GroupBy(it => it.EquipmentId.Split('_')[0]).
                        Select(it => new { EqId = it.Key, Time = it.Max(y => y.EventTime) }).ToList();
                    foreach (var item in allEqidMaxTime)
                    {
                        if (item.Time.AddSeconds(noLogIdleTime) < DateTime.Now)
                        {
                            log.Info($"{item.EqId} 最新状态时间为{item.Time},超过{noLogIdleTime}秒无状态，插入一个Idle");
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
                            channel.BasicPublish("", "EAP.Services", properties, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(trans)));
                        }
                    }



                    channel.Close();
                    connection.Close();

                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }



            }, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(interval));

            return Task.CompletedTask;
        }
    }
}
