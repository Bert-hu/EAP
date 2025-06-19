using EAP.Client.Model.Database;
using EAP.Client.RabbitMq;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Windows.Forms.AxHost;

namespace EAP.Client.Http
{
    class PanasonicDataHandleService : BackgroundService
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");

        private readonly IConfiguration configuration;
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly RabbitMqService rabbitMqService;

        public PanasonicDataHandleService(IConfiguration configuration, ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService)
        {
            this.configuration = configuration;
            this.sqlSugarClient = sqlSugarClient;
            this.rabbitMqService = rabbitMqService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    traLog.Info("开始上传设备数据到EAP");
                    var machineNoList = configuration.GetSection("Custom:MachineNoList").Get<Dictionary<string, string>>();
                    var statusDict = configuration.GetSection("Others:ProcessStateCodes").Get<Dictionary<string, string>>();
                    var equipmentType = configuration.GetSection("Custom")["EquipmentType"];

                    foreach (var item in machineNoList)
                    {
                        var machine = sqlSugarClient.Queryable<MachineConfig>().InSingle(item.Value);
                        if (machine == null)
                        {
                            machine = new MachineConfig
                            {
                                MachineId = item.Value,
                            };
                            sqlSugarClient.Insertable<MachineConfig>(machine).ExecuteCommand();
                        }

                        var lastUploadEventTime = machine.LastUploadEventTime;
                        var eventData = sqlSugarClient.Queryable<PanasonicEventData>().Where(it => it.MachineNo == item.Key && it.EventTime > lastUploadEventTime).ToList();
                        foreach (var _event in eventData)
                        {
                            var lastStatus = string.Empty;
                            //Status
                            var status = statusDict.FirstOrDefault(it => (_event.Message + "_" + _event.SubMessage).Contains(it.Key)).Value;
                            if (status != null && status != lastStatus)
                            {
                                var para = new Dictionary<string, object>
                                {
                                    { "EQID",machine.MachineId.Split('_')[0] },
                                    { "DateTime",_event.EventTime},
                                    { "EQType",equipmentType },
                                    { "Status",status}
                                };
                                RabbitMqTransaction trans = new RabbitMqTransaction
                                {
                                    TransactionName = "EquipmentStatus",
                                    Parameters = para
                                };
                                rabbitMqService.Produce("EAP.Services", trans);
                                lastStatus = status;
                            }

                            //Alarm
                            if ((_event.Message + "_" + _event.SubMessage).Contains("报警_"))
                            {
                                var para = new Dictionary<string, object>
                                {
                                    { "AlarmEqp", machine.MachineId.Split('_')[0]},
                                    { "AlarmCode",_event.SubCode},
                                    { "AlarmText",_event.SubMessage},
                                    { "AlarmSource", equipmentType},
                                    { "AlarmTime",_event.EventTime},
                                    { "AlarmSet",true}
                                };
                                var trans = new RabbitMqTransaction
                                {
                                    TransactionName = "EquipmentAlarm",
                                    Parameters = para,
                                };
                                rabbitMqService.Produce("EAP.Services", trans);
                            }

                        }
                        if (eventData.Count > 0)
                        {
                            machine.LastUploadEventTime = eventData.Max(it => it.EventTime);
                            sqlSugarClient.Updateable<MachineConfig>(machine).ExecuteCommand();
                        }
                    }
                    traLog.Info("上传数据完成");
                }
                catch (Exception ex)
                {
                    traLog.Error(ex.ToString());
                }
                await Task.Delay(10000);
            }
        }

    }
}
