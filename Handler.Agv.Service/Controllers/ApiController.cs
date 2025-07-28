using HandlerAgv.Service.Models;
using HandlerAgv.Service.Models.Database;
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
        /// 查询各个机种Handler设备开机数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMachineCountByMaterial()
        {
            var eq = sqlSugarClient.Queryable<HandlerEquipmentStatus>().ToList();

            var eqids = sqlSugarClient.Queryable<HandlerEquipmentStatus>()
                .Where(it => it.AgvEnabled == true)
                .GroupBy(it => new { it.MaterialName, it.GroupName })
                .Select(it => new
                {
                    MaterialName = it.MaterialName,
                    GroupName = it.GroupName,
                    Count = SqlFunc.AggregateCount(it.Id)
                }).ToList();
            return Json(eqids);
        }


    }
}
