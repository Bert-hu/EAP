using AutoMapper;
using HandlerAgv.Service.Extension;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Models.ViewModel;
using HandlerAgv.Service.RabbitMq;
using log4net;
using Newtonsoft.Json;
using SqlSugar;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HandlerAgv.Service.Services
{
    public class AgvApiService
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;
        private readonly DbConfigurationService dbConfiguration;
        private readonly RabbitMqService rabbitMqService;

        public AgvApiService(ISqlSugarClient sqlSugarClient, IMapper mapper, DbConfigurationService dbConfiguration, RabbitMqService rabbitMqService)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
            this.dbConfiguration = dbConfiguration;
            this.rabbitMqService = rabbitMqService;
        }

        public async Task<(bool, string)> SendInputOutputTask(HandlerEquipmentStatus equipment)
        {
            try
            {
                var agvTaskRequest = new AgvTaskRequest
                {
                    TaskType = AgvTaskType.InputOutput.ToString(),
                    EQID = equipment.Id,
                    MaterialName = equipment.MaterialName,
                    GroupName = equipment.GroupName,
                    OutputLot = equipment.CurrentLot
                };
                var agvApiUrl = dbConfiguration.GetConfigurations("AgvApiUrl")?.TrimEnd('/');
                var method = "/api/v3/handler/eap/order";
                dbgLog.Info($"agv request:{agvApiUrl + method},{JsonConvert.SerializeObject(agvTaskRequest)}");
                var agvTaskResponse = await HttpClientHelper.HttpPostRequestAsync<AgvTaskRequest>(agvApiUrl + method, agvTaskRequest);
                dbgLog.Info($"agv response:{JsonConvert.SerializeObject(agvTaskResponse)}");

                //if (agvTaskResponse.Result)
                {
                    var task = new HandlerAgvTask
                    {
                        ID = agvTaskRequest.TaskId,
                        Type = AgvTaskType.InputOutput,
                        EquipmentId = equipment.Id,
                        MaterialName = equipment.MaterialName,
                        GroupName = equipment.GroupName,
                        Status = AgvTaskStatus.AgvRequested,
                        AgvRequestTime = DateTime.Now,
                    };
                    await sqlSugarClient.Insertable(task).ExecuteCommandAsync();
                    equipment.CurrentTaskId = task.ID;
                    await sqlSugarClient.Updateable(equipment).UpdateColumns(it => new { it.CurrentTaskId }).ExecuteCommandAsync();
                    EapClientService eapClient = new EapClientService(sqlSugarClient, rabbitMqService);
                    eapClient.UpdateClientInfo(equipment.Id);
                    return (true, string.Empty);
                }
                //else
                //{
                //    dbgLog.Error($"AGV任务请求失败: {agvTaskResponse.Message}");
                //    return (false, "AGV任务请求失败:" + agvTaskResponse.Message);
                //}

            }
            catch (TaskCanceledException)
            {
                dbgLog.Error("请求AGV超时.");
                return (false, "请求AGV超时.");
            }
            catch (Exception ex)
            {
                dbgLog.Error($"Request failed: {ex.ToString()}");
                return (false, "请求失败:" + ex.Message);
            }
        }

        public async Task<(bool, string)> SendInputTask(HandlerEquipmentStatus equipment)
        {
            try
            {
                var agvTaskRequest = new AgvTaskRequest { TaskType = AgvTaskType.Input.ToString(), EQID = equipment.Id, MaterialName = equipment.MaterialName, GroupName = equipment.GroupName };
                var agvApiUrl = dbConfiguration.GetConfigurations("AgvApiUrl")?.TrimEnd('/');
                var method = "/api/v3/handler/eap/order";
                dbgLog.Info($"agv request:{agvApiUrl + method},{JsonConvert.SerializeObject(agvTaskRequest)}");
                var agvTaskResponse = await HttpClientHelper.HttpPostRequestAsync<AgvTaskRequest>(agvApiUrl + method, agvTaskRequest);
                dbgLog.Info($"agv response:{JsonConvert.SerializeObject(agvTaskResponse)}");

                //if (agvTaskResponse.Result)
                {
                    var task = new HandlerAgvTask
                    {
                        ID = agvTaskRequest.TaskId,
                        Type = AgvTaskType.Input,
                        EquipmentId = equipment.Id,
                        MaterialName = equipment.MaterialName,
                        GroupName = equipment.GroupName,
                        Status = AgvTaskStatus.AgvRequested,
                        AgvRequestTime = DateTime.Now,
                    };
                    await sqlSugarClient.Insertable(task).ExecuteCommandAsync();
                    equipment.CurrentTaskId = task.ID;
                    await sqlSugarClient.Updateable(equipment).UpdateColumns(it => new { it.CurrentTaskId }).ExecuteCommandAsync();
                    EapClientService eapClient = new EapClientService(sqlSugarClient, rabbitMqService);
                    eapClient.UpdateClientInfo(equipment.Id);
                    return (true, string.Empty);
                }
                //else
                //{
                //    dbgLog.Error($"AGV任务请求失败: {agvTaskResponse.Message}");
                //    return (false, "AGV任务请求失败:" + agvTaskResponse.Message);
                //}
            }
            catch (TaskCanceledException)
            {
                dbgLog.Error("请求AGV超时.");
                return (false, "请求AGV超时.");
            }
            catch (Exception ex)
            {
                dbgLog.Error($"Request failed: {ex.ToString()}");
                return (false, "请求失败:" + ex.Message);
            }
        }

        public async Task<(bool, string)> SendOutputTask(HandlerEquipmentStatus equipment)
        {
            try
            {
                var agvTaskRequest = new AgvTaskRequest
                {
                    TaskType = AgvTaskType.Output.ToString(),
                    EQID = equipment.Id,
                    MaterialName = equipment.MaterialName,
                    GroupName = equipment.GroupName,
                    OutputLot = equipment.CurrentLot
                };
                var agvApiUrl = dbConfiguration.GetConfigurations("AgvApiUrl")?.TrimEnd('/');
                var method = "/api/v3/handler/eap/order";
                dbgLog.Info($"agv request:{agvApiUrl + method},{JsonConvert.SerializeObject(agvTaskRequest)}");
                var agvTaskResponse = await HttpClientHelper.HttpPostRequestAsync<AgvTaskRequest>(agvApiUrl + method, agvTaskRequest);
                dbgLog.Info($"agv response:{JsonConvert.SerializeObject(agvTaskResponse)}");

                //if (agvTaskResponse.Result)
                {
                    var task = new HandlerAgvTask
                    {
                        ID = agvTaskRequest.TaskId,
                        Type = AgvTaskType.Output,
                        EquipmentId = equipment.Id,
                        MaterialName = equipment.MaterialName,
                        GroupName = equipment.GroupName,
                        Status = AgvTaskStatus.AgvRequested,
                        AgvRequestTime = DateTime.Now,
                    };
                    await sqlSugarClient.Insertable(task).ExecuteCommandAsync();
                    equipment.CurrentTaskId = task.ID;
                    await sqlSugarClient.Updateable(equipment).UpdateColumns(it => new { it.CurrentTaskId }).ExecuteCommandAsync();
                    EapClientService eapClient = new EapClientService(sqlSugarClient, rabbitMqService);
                    eapClient.UpdateClientInfo(equipment.Id);
                    return (true, string.Empty);
                }
                //else
                //{
                //    dbgLog.Error($"AGV任务请求失败: {agvTaskResponse.Message}");
                //    return (false, "AGV任务请求失败:" + agvTaskResponse.Message);
                //}

            }
            catch (TaskCanceledException)
            {
                dbgLog.Error("请求AGV超时.");
                return (false, "请求AGV超时.");
            }
            catch (Exception ex)
            {
                dbgLog.Error($"Request failed: {ex.ToString()}");
                return (false, "请求失败:" + ex.Message);
            }
        }

    }
}
