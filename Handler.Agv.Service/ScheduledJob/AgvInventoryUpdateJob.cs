using AutoMapper;
using HandlerAgv.Service.Models;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Models.Inventory;
using HandlerAgv.Service.RabbitMq;
using HandlerAgv.Service.Services;
using ICOSEAP.Api.Services;
using log4net;
using Microsoft.Extensions.Configuration;
using Quartz;
using SqlSugar;
using System.Configuration;
using System.Text;

namespace HandlerAgv.Service.ScheduledJob
{
    [DisallowConcurrentExecution]

    public class AgvInventoryUpdateJob : IJob
    {
        private static log4net.ILog Log = LogManager.GetLogger("Debug");

        public async Task Execute(IJobExecutionContext context)
        {
            var configuration = context.JobDetail.JobDataMap["configuration"] as IConfiguration;
            var dbConfiguration = context.JobDetail.JobDataMap["dbConfiguration"] as DbConfigurationService;
            var mapper = context.JobDetail.JobDataMap["mapper"] as IMapper;
            var rabbitMqService = context.JobDetail.JobDataMap["rabbitMqService"] as RabbitMqService;


            var sqlSugarClient = SqlsugarService.GetSqlSugarClient(configuration);


            try
            {
                var machines = sqlSugarClient.Queryable<HandlerEquipmentStatus>().ToList();
                var invs = machines.Where(it => !string.IsNullOrEmpty(it.MaterialName) && !string.IsNullOrEmpty(it.GroupName) && it.IsValiad).GroupBy(it => new { it.MaterialName, it.GroupName })
                    .Select(g => new HandlerInventory { MaterialName = g.Key.MaterialName, GroupName = g.Key.GroupName, EnableMachineCount = g.Count(a => a.AgvEnabled && a.IsValiad) }).ToList();
                AgvApiService agvApiService = new AgvApiService(sqlSugarClient, mapper, dbConfiguration, rabbitMqService);
                //AGV库存
                var agvInventories = await agvApiService.GetAgvInventories();
                if (agvInventories != null)
                {
                    invs.ForEach(it =>
                    {
                        it.AgvQuantity = agvInventories.Where(inv => inv.materialName == it.MaterialName && inv.groupName == it.GroupName).Count();
                    });
                }
                else
                {
                    invs.ForEach(it => { it.AgvQuantity = -1; });
                }
                //Stocker1库存
                var stocker1Invs = sqlSugarClient.Queryable<StockerInventory_I>().ToList();
                if (stocker1Invs != null && stocker1Invs.Count > 0)
                {
                    invs.ForEach(it =>
                    {
                        it.Stocker1Quantity = stocker1Invs.Where(inv => (inv.MODELNAME?.Contains(it.MaterialName) ?? false) && inv.GROUPNAME == it.GroupName && (inv.STATUS == "2"||inv.STATUS =="10")).Count();
                    });
                }
                //Stocker2库存
                var stocker2Invs = sqlSugarClient.Queryable<StockerInventory_II>().ToList();
                if (stocker2Invs != null && stocker2Invs.Count > 0)
                {
                    invs.ForEach(it =>
                    {

                        it.Stocker2Quantity = stocker2Invs.Where(inv =>( inv.MODELNAME?.Contains(it.MaterialName)??false) && inv.GROUPNAME == it.GroupName && (inv.STATUS == "2" || inv.STATUS == "10")).Count();
                    });
                }
                sqlSugarClient.BeginTran();
                sqlSugarClient.Deleteable<HandlerInventory>().ExecuteCommand();
                sqlSugarClient.Insertable<HandlerInventory>(invs).ExecuteCommand();
                sqlSugarClient.CommitTran();
            
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            sqlSugarClient.Dispose();
        }
    }
}
