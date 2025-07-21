using LaserMonitor.Service.Models;
using LaserMonitor.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using SqlSugar;
using System.Drawing;

namespace LaserMonitor.Service.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ISqlSugarClient sqlSugarClient;

        public DashboardController(ISqlSugarClient sqlSugarClient)
        {
            this.sqlSugarClient = sqlSugarClient;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetMachineList()
        {
            ConfigManager<MonitoringConfig> manager = new ConfigManager<MonitoringConfig>();
            var config = manager.LoadConfig();
            var eqids = config.EquipmentMonitorTime.Keys.ToList();
            return Json(eqids);
        }

        [HttpPost]
        public JsonResult GetConfigs()
        {
            ConfigManager<MonitoringConfig> manager = new ConfigManager<MonitoringConfig>();
            var config = manager.LoadConfig();
            var eqids = config.EquipmentMonitorTime.Keys.ToList();

            return Json(new { eqids = eqids, lowerlimit = config.LowerLimit, upperlimit = config.UpperLimit });
        }

        public JsonResult SetLimits(string equipmentid, string lowerlimit, string upperlimit)
        {
            ConfigManager<MonitoringConfig> manager = new ConfigManager<MonitoringConfig>();
            var config = manager.LoadConfig();

            var hasSpecificConfig = config.EquipmentConfig.TryGetValue(equipmentid, out SpecificConfig specificConfig);
            if (specificConfig != null)
            {
                specificConfig.UpperLimit = double.Parse(upperlimit);
                specificConfig.LowerLimit = double.Parse(lowerlimit);
            }
            else
            {
                specificConfig = new SpecificConfig();
                specificConfig.UpperLimit = double.Parse(upperlimit);
                specificConfig.LowerLimit = double.Parse(lowerlimit);
                config.EquipmentConfig.Add(equipmentid, specificConfig);
            }
            //config.UpperLimit = double.Parse(upperlimit);
            //config.LowerLimit = double.Parse(lowerlimit);
            manager.SaveConfig(config);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult GetData(string equipmentid, DateTime startTime, DateTime endTime)
        {
            var data = sqlSugarClient.Queryable<EquipmentParamsHisRaw>()
                .Where(it => it.EQID == equipmentid && it.NAME == "Power" && it.UPDATETIME > startTime && it.UPDATETIME < endTime).OrderBy(it => it.UPDATETIME).ToList();
            ConfigManager<MonitoringConfig> manager = new ConfigManager<MonitoringConfig>();
            var config = manager.LoadConfig();
            var upperLimit = config.UpperLimit;
            var lowerLimit = config.LowerLimit;

            var hasSpecificConfig = config.EquipmentConfig.TryGetValue(equipmentid, out SpecificConfig specificConfig);
            if (hasSpecificConfig)
            {
                upperLimit = specificConfig.UpperLimit;
                lowerLimit = specificConfig.LowerLimit;
            }


            var chartdata = data.Select(it => new
            {
                value = new List<object> { it.UPDATETIME, it.VALUE, it.EQID },
                itemStyle = new
                {
                    normal = new
                    {
                        color = double.Parse(it.VALUE) < lowerLimit || double.Parse(it.VALUE) > upperLimit ? "red" : "lightgreen"
                    }
                }
            }).ToList();
            var temp = data.Select(it => double.Parse(it.VALUE)).ToList();
            temp.Add(lowerLimit);
            temp.Add(upperLimit);
            var (min, max) = CalculateYAxisRange(temp);

            var useageData = sqlSugarClient.Queryable<EquipmentParamsHisRaw>().Where(it => it.EQID == equipmentid && it.NAME == "UsageDuration" && it.UPDATETIME > startTime && it.UPDATETIME < endTime).OrderBy(it => it.UPDATETIME).ToList();
            var useageChartData = useageData.Select(it => new
            {
                value = new List<object> { it.UPDATETIME, it.VALUE, it.EQID }
            }).ToList();


            return Json(new { chartdata = chartdata, min, max, lowerlimit = lowerLimit, upperlimit = upperLimit, useageChartData = useageChartData });
        }


        private (double min, double max) CalculateYAxisRange(IEnumerable<double> values)
        {
            if (!values.Any())
            {
                return (0, 10); // 如果没有数据，默认范围 0 到 10
            }

            double actualMin = values.Min();
            double actualMax = values.Max();

            // 计算最佳显示范围
            double range = actualMax - actualMin;
            double padding = range * 0.1; // 预留 10% 的空间

            double min = Math.Floor(actualMin - padding);
            double max = Math.Ceiling(actualMax + padding);

            return (min, max);
        }

        [HttpPost]
        public ActionResult DownloadData(string equipmentid, DateTime startTime, DateTime endTime)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    // 创建工作表
                    var worksheet = package.Workbook.Worksheets.Add("Data");

                    var data = sqlSugarClient.Queryable<EquipmentParamsHisRaw>()
                        .Where(it => it.NAME == "Power" && it.UPDATETIME > startTime && it.UPDATETIME < endTime).OrderBy(it => it.UPDATETIME).ToList();

                    // 设置列标题
                    worksheet.Cells[1, 1].Value = "EQID"; // EQID
                    worksheet.Cells[1, 2].Value = "Time"; // UPDATETIME
                    worksheet.Cells[1, 3].Value = "Power(W)";


                    // 写入数据
                    for (int i = 0; i < data.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = data[i].EQID; // ID
                        worksheet.Cells[i + 2, 2].Value = data[i].UPDATETIME; // LINE
                        worksheet.Cells[i + 2, 2].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                        worksheet.Cells[i + 2, 3].Value = data[i].VALUE; // 创建时间
                    }

                    var worksheet1 = package.Workbook.Worksheets.Add("UsageDuration");

                    var data1 = sqlSugarClient.Queryable<EquipmentParamsHisRaw>()
                        .Where(it => it.NAME == "UsageDuration" && it.UPDATETIME > startTime && it.UPDATETIME < endTime).OrderBy(it => it.UPDATETIME).ToList();

                    // 设置列标题
                    worksheet1.Cells[1, 1].Value = "EQID"; // EQID
                    worksheet1.Cells[1, 2].Value = "Time"; // UPDATETIME
                    worksheet1.Cells[1, 3].Value = "UsageDuration(s)";

                    // 写入数据
                    for (int i = 0; i < data1.Count; i++)
                    {
                        worksheet1.Cells[i + 2, 1].Value = data1[i].EQID; // ID
                        worksheet1.Cells[i + 2, 2].Value = data1[i].UPDATETIME; // LINE
                        worksheet1.Cells[i + 2, 2].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                        worksheet1.Cells[i + 2, 3].Value = data1[i].VALUE; // 创建时间
                    }



                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "data.xlsx");
                }
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
    }
}
