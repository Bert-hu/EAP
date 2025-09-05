using AutoMapper;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Services;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class SendInputOutputTask : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        private readonly RabbitMqService rabbitMqService;
        private readonly IMapper mapper;
        private readonly DbConfigurationService dbConfiguration;
        public SendInputOutputTask(ISqlSugarClient sqlSugarClient, RabbitMqService rabbitMqService, IMapper mapper, DbConfigurationService dbConfiguration)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.rabbitMqService = rabbitMqService;
            this.mapper = mapper;
            this.dbConfiguration = dbConfiguration;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var repTrans = trans.GetReplyTransaction();
            try
            {
                var machine = sqlSugarClient.Queryable<HandlerEquipmentStatus>().InSingle(trans.EquipmentID);
                if (machine == null)
                {
                    repTrans.Parameters.Add("Result", false);
                    repTrans.Parameters.Add("Message", "Machine not found");
                }
                else
                {
                    if (machine.IsValiad && machine.AgvEnabled)
                    {
                        if (string.IsNullOrEmpty(machine.CurrentTaskId))
                        {
                            AgvApiService agvApiService = new AgvApiService(sqlSugarClient, mapper, dbConfiguration, rabbitMqService);
                            var (result, message) =await agvApiService.SendInputOutputTask(machine);
                            if (result)
                            {
                                repTrans.Parameters.Add("Result", true);
                                repTrans.Parameters.Add("Message", "InputOutput任务发送成功");
                                dbgLog.Info($"InputOutput任务发送成功： {machine.Id}");

                                EapClientService eapClient = new EapClientService(sqlSugarClient,rabbitMqService);
                                eapClient.UpdateClientInfo(trans.EquipmentID, "InputOutput任务发送成功");
                            }
                            else
                            {
                                repTrans.Parameters.Add("Result", false);
                                repTrans.Parameters.Add("Message", message);
                            }
                        }
                        else
                        { 
                            repTrans.Parameters.Add("Result", false);
                            repTrans.Parameters.Add("Message", $"{machine.Id} 当前已有任务在执行，无法发送InputOutput任务。");
                            dbgLog.Warn($"{machine.Id} 当前已有任务在执行，无法发送InputOutput任务。");
                        }   
                    }
                    else
                    {
                        repTrans.Parameters.Add("Result", false);
                        repTrans.Parameters.Add("Message", $"{machine.Id} 未启用或AGV功能未开。");
                        dbgLog.Warn($"Machine {machine.Id} is not valid or AGV is not enabled.");
                    }
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.Message, ex);
                repTrans.Parameters.Add("Result", false);
                repTrans.Parameters.Add("Message", "发送InputOutput任务异常: " + ex.Message);
            }
            rabbitMqService.Produce(trans.ReplyChannel, repTrans);
        }
    }
}
