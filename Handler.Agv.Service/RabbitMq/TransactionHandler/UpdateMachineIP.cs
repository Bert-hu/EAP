using HandlerAgv.Service.Models.Database;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace HandlerAgv.Service.RabbitMq.TransactionHandler
{
    public class UpdateMachineIP : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        private ISqlSugarClient sqlSugarClient;
        public UpdateMachineIP(ISqlSugarClient sqlSugarClient)
        {
            this.sqlSugarClient = sqlSugarClient;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            try
            {
                string? ipAddress = trans.Parameters["MachineIP"]?.ToString();
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    var machine = sqlSugarClient.Queryable<HandlerEquipmentStatus>().InSingle(trans.EquipmentID);
                    if (machine == null)
                    {
                        machine = new HandlerEquipmentStatus
                        {
                            Id = trans.EquipmentID,
                            UpdateTime = DateTime.Now,
                            IP = ipAddress
                        };
                        await sqlSugarClient.Insertable(machine).ExecuteCommandAsync();
                    }
                    else
                    {
                        machine.IP = ipAddress;
                        await sqlSugarClient.Updateable(machine)
                            .UpdateColumns(it => new { it.IP })
                            .ExecuteCommandAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.Message, ex);
            }
        }
    }
}
