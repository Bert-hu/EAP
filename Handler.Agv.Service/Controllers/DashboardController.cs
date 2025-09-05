using AutoMapper;
using HandlerAgv.Service.Models;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.RabbitMq;
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
        private readonly IMapper mapper;
        private readonly DbConfigurationService dbConfiguration;
        private readonly RabbitMqService rabbitMqService;

        public DashboardController(ISqlSugarClient sqlSugarClient, IMapper mapper, DbConfigurationService dbConfiguration, RabbitMqService rabbitMqService)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
            this.dbConfiguration = dbConfiguration;
            this.rabbitMqService = rabbitMqService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAgvData()
        {
            AgvApiService agvApi = new AgvApiService(sqlSugarClient, mapper, dbConfiguration, rabbitMqService);
            var data = agvApi.GetAgvStatus().Result;

            return Json(data);
        }

        [HttpGet]

        public JsonResult GetInventoryStatus()
        {
            var inventory = sqlSugarClient.Queryable<HandlerInventory>().Where(it => it.UpdateTime > DateTime.Now.AddMinutes(-2)).ToList();
            return Json(inventory);
        }

    }
}
