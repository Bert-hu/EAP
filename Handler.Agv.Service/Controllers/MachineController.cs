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
    [HiddenApi]
    [Route("[controller]/[action]")]

    public class MachineController : Controller
    {
        private readonly ISqlSugarClient sqlSugarClient;

        public MachineController(ISqlSugarClient sqlSugarClient)
        {
            this.sqlSugarClient = sqlSugarClient;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetMachineData(int? page, int? limit,string? searchText)
        {
            var list = sqlSugarClient.Queryable<HandlerEquipmentStatus>().ToList();
            return new JsonResult(new { code =0, data = list, count=list.Count });
        }

     
    }
}
