using LaserMonitor.Service.Models;
using LaserMonitor.Service.Services;
using log4net;
using Microsoft.Extensions.Configuration;
using Quartz;
using System.Configuration;
using System.Text;

namespace LaserMonitor.Service.ScheduledJob
{
    public class LaserPowerMonitorJob : IJob
    {
        private static log4net.ILog Log = LogManager.GetLogger("Debug");
        private IConfiguration configuration;

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                configuration = context.JobDetail.JobDataMap["configuration"] as IConfiguration;
                var sqlSugarClient = SqlsugarService.GetSqlSugarClient(configuration);

                ConfigManager<MonitoringConfig> manager = new ConfigManager<MonitoringConfig>();
                var config = manager.LoadConfig();
                var eqids = config.EquipmentMonitorTime.Keys.ToList();
                foreach (var eqid in eqids)
                {
                    config = manager.LoadConfig();
                    var lasttime = config.EquipmentMonitorTime[eqid];
                    var data = sqlSugarClient.Queryable<EquipmentParamsHisRaw>().Where(it => it.EQID == eqid && it.NAME == "Power" && it.UPDATETIME > lasttime).OrderBy(it => it.UPDATETIME).ToList();
                    if (data.Count > 0)
                    {
                        config.EquipmentMonitorTime[eqid] = data[data.Count - 1].UPDATETIME;
                        manager.SaveConfig(config);

                        var upperLimit = config.UpperLimit;
                        var lowerLimit = config.LowerLimit;

                        var hasSpecificConfig = config.EquipmentConfig.TryGetValue(eqid, out SpecificConfig specificConfig);
                        if (hasSpecificConfig)
                        {
                            upperLimit = specificConfig.UpperLimit;
                            lowerLimit = specificConfig.LowerLimit;
                        }


                        var oocData = data.Where(it => double.Parse(it.VALUE) > upperLimit || double.Parse(it.VALUE) < lowerLimit).ToList();
                        if (oocData.Count > 0)
                        {
                            //记录到Log
                            foreach (var item in oocData)
                            {
                                Log.Info(item.EQID + " " + item.NAME + " " + item.UPDATETIME + " " + item.VALUE);
                            }
                            SendMailToUser(config.EmailList, oocData, lowerLimit, upperLimit);

                            if (config.PhoneList.Count > 0)
                            {
                                var message = $"设备{eqid}的功率{oocData.First().UPDATETIME}超出设定上下限:{oocData.First().VALUE}";
                                ShortMessageHelper.SendMsgToPhoneList("LaserPowerOOC", message, config.PhoneList);
                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return Task.CompletedTask;
        }

        private void SendMailToUser(List<string> maillist, List<EquipmentParamsHisRaw> data, double lowerlimit, double upperlimit)
        {
            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.Append($"<p>以下数据超出设定上下限,下限：{lowerlimit}，上限：{upperlimit}");
            tableBuilder.Append("<table class=\"styled-table\">");
            tableBuilder.Append("<tr><th>EQID</th><th>时间</th><th>Power（W）</th></tr>");
            foreach (var item in data)
            {
                tableBuilder.Append($"<tr><td>{item.EQID}</td><td>{item.UPDATETIME}</td><td>{item.VALUE}</td></tr>");
            }
            tableBuilder.Append("</table>");
            string tableStyle = @"
    <style>
        .styled-table {
            border-collapse: collapse;
            font-size: 14px;
            font-family: Arial, sans-serif;
            min-width: 100%;
            width: auto;
            white-space: nowrap;
            background-color: #fff;
            color: #000;
            border: 1px solid #ccc;
        }
        .styled-table td, .styled-table th {
            padding: 12px 15px;
            border-right: 1px solid #ccc;
            text-align: left;
        }
        .styled-table th {
            background-color: #eee;
            color: #000;
            border: 1px solid #ccc;
            font-weight: bold;
        }
    </style>
";
            string content = tableStyle + tableBuilder.ToString();
            MailHelper.SendMail(maillist.ToArray(), "Laser Power OOC", content);
        }
    }
}
