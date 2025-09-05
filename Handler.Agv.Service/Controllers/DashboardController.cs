using AutoMapper;
using HandlerAgv.Service.Models;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Models.ViewModel;
using HandlerAgv.Service.RabbitMq;
using HandlerAgv.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using SqlSugar;
using System.Drawing;
using System.Xml.Linq;

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
            data = data?.Where(it => it.nickname.Contains("机械臂")).OrderBy(it => it.nickname).ToList();

            return Json(data);
        }

        [HttpGet]
        public JsonResult GetInventoryStatus()
        {
            var inventory = sqlSugarClient.Queryable<HandlerInventory>().Where(it => it.UpdateTime > DateTime.Now.AddMinutes(-2)).ToList();
            return Json(inventory);
        }

        [HttpGet]
        public JsonResult GetHandlerStatus()
        {
            var dbData = sqlSugarClient.Queryable<HandlerEquipmentStatus>().Where(it => it.IsValiad).OrderBy(it => it.Id).ToList();
            var vmData = mapper.Map<List<HandlerEquipmentStatusVm>>(dbData);
            vmData.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.CurrentTaskId))
                {
                    var task = sqlSugarClient.Queryable<HandlerAgvTask>()
                        .Where(t => t.ID == x.CurrentTaskId)
                        .First();
                    if (task != null)
                    {
                        x.CurrentTaskRequestTime = task.AgvRequestTime;
                        switch (task.Status)
                        {
                            case AgvTaskStatus.AgvRequested:
                                x.CurrentTaskState = "AGV任务已请求";
                                break;
                            case AgvTaskStatus.AgvArrived:
                                x.CurrentTaskState = "AGV已到达";
                                break;
                            case AgvTaskStatus.MachineReady:
                                x.CurrentTaskState = "设备已锁定进出料";
                                break;
                            case AgvTaskStatus.AgvRobotFinished:
                                x.CurrentTaskState = "AGV手臂任务已完成";
                                break;
                            default:
                                x.CurrentTaskState = "未知状态";
                                break;
                        }
                    }
                }
                else
                {
                    x.CurrentTaskState = "无AGV任务";
                }
            });
            return Json(vmData);
        }

        [HttpGet]
        public JsonResult GetTaskStats()
        {
            var fromData = DateTime.Today.AddDays(-14);
            var dbData = sqlSugarClient.Queryable<HandlerAgvTask>().Where(it => it.AgvRequestTime > fromData && (it.Status == AgvTaskStatus.Completed || it.Status == AgvTaskStatus.AbnormalEnd)).ToList();
            //按照日期分组，统计每个日期完成的数量,成功率
            var vmData = dbData.GroupBy(it => ((DateTime)it.AgvRequestTime).ToString("M-d")).Select(g => new
            {
                date = g.Key,
                count = g.Count(),
                successRate = (double)g.Count(x => x.Status == AgvTaskStatus.Completed) / g.Count() * 100
            }).OrderBy(it => it.date).ToList();

            return Json(vmData);
        }

    }
}
