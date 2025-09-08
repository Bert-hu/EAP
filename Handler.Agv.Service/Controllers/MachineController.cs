using AutoMapper;
using HandlerAgv.Service.Models;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Models.Inventory;
using HandlerAgv.Service.Models.ViewModel;
using HandlerAgv.Service.RabbitMq;
using HandlerAgv.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using SqlSugar;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HandlerAgv.Service.Controllers
{
    [HiddenApi]
    [Route("[controller]/[action]")]

    public class MachineController : Controller
    {
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;
        private readonly RabbitMqService rabbitMQService;

        public MachineController(ISqlSugarClient sqlSugarClient, IMapper mapper, RabbitMqService rabbitMQService)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
            this.rabbitMQService = rabbitMQService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetMachineData(int? page, int? limit, string? searchText)
        {
            var dbData = sqlSugarClient.Queryable<HandlerEquipmentStatus>().OrderBy(it => it.Id).ToList();
            if (!string.IsNullOrEmpty(searchText))
            {
                dbData = dbData.Where(x => (x.Id + x.RecipeName + x.MaterialName + x.GroupName).ToUpper().Contains(searchText.ToUpper())).ToList();
            }
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
            var inventory = sqlSugarClient.Queryable<HandlerInventory>().Where(it => it.UpdateTime > DateTime.Now.AddMinutes(-2)).ToList();
            return new JsonResult(new { code = 0, data = vmData, count = vmData.Count, inventory = inventory });
        }


        [HttpPost]
        public JsonResult UpdateMachineData(HandlerEquipmentStatus data)
        {

            var count = sqlSugarClient.Updateable(data).UpdateColumns(it => new { it.AgvEnabled, it.MaterialName, it.GroupName, it.InputTrayNumber, it.OutputTrayNumber, it.CurrentTaskId }).ExecuteCommand();
            if (data.InputTrayNumber > 0)
            {
                data.LoaderEmpty =false;
                sqlSugarClient.Updateable(data).UpdateColumns(it => new { it.LoaderEmpty }).ExecuteCommand();
            }
            EapClientService clientService = new EapClientService(sqlSugarClient, rabbitMQService);
            clientService.UpdateClientInfo(data.Id, $"数据在管理页面被更新");
            return new JsonResult(new { code = count == 1 });

        }
        [HttpGet]
        public JsonResult GetMaterialData()
        {
            var dbData = sqlSugarClient.Queryable<StockerInventory_II>().ToList();

            var stocker1data = sqlSugarClient.Queryable<StockerInventory_I>().ToList();
            return new JsonResult(new { code = 0, data = dbData });
        }

        [HttpGet]
        public JsonResult GetMachineTaskData(int page, int limit, string equipmentId)
        {
            var total = 0;
            var dbData = sqlSugarClient.Queryable<HandlerAgvTask>()
                .Where(it => it.EquipmentId == equipmentId).OrderByDescending(it => it.AgvRequestTime).ToPageList(page, limit, ref total);
            return new JsonResult(new { code = 0, data = dbData, count = total });
        }

    }
}
