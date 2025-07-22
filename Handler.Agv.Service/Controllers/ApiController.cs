using HandlerAgv.Service.Models;
using HandlerAgv.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using SqlSugar;
using System.Drawing;

namespace HandlerAgv.Service.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ApiController : Controller
    {
        private readonly ISqlSugarClient sqlSugarClient;

        public ApiController(ISqlSugarClient sqlSugarClient)
        {
            this.sqlSugarClient = sqlSugarClient;
        }

        /// <summary>
        /// 获取设备清单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMachineList()
        {
            ConfigManager<MonitoringConfig> manager = new ConfigManager<MonitoringConfig>();
            var config = manager.LoadConfig();
            var eqids = config.EquipmentMonitorTime.Keys.ToList();
            return Json(eqids);
        }


    }
}
