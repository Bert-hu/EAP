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
    [HiddenApi]
    [Route("[controller]/[action]")]

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

     
    }
}
