using AutoMapper;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Models.ViewModel;
using log4net;
using SqlSugar;

namespace ICOSEAP.Api.Services
{
    public class MachineEstimatedService
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug_Pd2");
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;

        public MachineEstimatedService(ISqlSugarClient sqlSugarClient, IMapper mapper)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
        }

        public void UpdateCycleTime(string equipmentId)
        {
            string sql = string.Format(@"WITH EventWithLag AS (
    SELECT 
        EH.""EquipmentId"",
        EH.""EventTime"",
        EH.""RecipeName"",
        EH.""BatchName"",
        EH.""EventName"",
        LAG(""EventTime"") OVER (PARTITION BY EH.""EquipmentId"", EH.""RecipeName"", EH.""BatchName"", EH.""EventName"" ORDER BY EH.""EventTime"") AS ""PreviousEventTime""
    FROM 
        ""HandlerEventHist"" EH, ""HandlerEquipmentStatus"" ES
    WHERE 
        ""EventTime"" > SYSDATE - 7
        AND ""EventName"" IN ('X1TrayStart', 'X3TrayComplete')  -- 只考虑这两个事件
        AND ES.""Id"" = EH.""EquipmentId""
        AND ES.""RecipeName"" = EH.""RecipeName""    --当前设备的当前recipe
        --AND EH.""RecipeName"" = '/recipe/2103-210201'  
        AND ES.""Id"" = '{0}'
    ORDER BY ""EventTime"" DESC
),
TimeDifferences AS (
    SELECT 
        ""EquipmentId"",
        ""RecipeName"",
        ""BatchName"",
        ""EventName"",
        ""EventTime"",
        ""PreviousEventTime"",
        ((""EventTime"" - ""PreviousEventTime"") * 86400) AS ""TimeDifference""  -- 转换为秒
    FROM 
        EventWithLag
    WHERE 
        ""PreviousEventTime"" IS NOT NULL
)
SELECT 
    ""EquipmentId"",
    ""RecipeName"" AS ""Recipe"",
    ROUND(PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY CASE WHEN ""EventName"" = 'X1TrayStart' THEN ""TimeDifference"" END), 0) AS ""InputTrayCT"", 
    ROUND(PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY CASE WHEN ""EventName"" = 'X3TrayComplete' THEN ""TimeDifference"" END), 0) AS ""OutputTrayCT"" 
FROM 
    TimeDifferences
GROUP BY 
    ""EquipmentId"", 
    ""RecipeName""
ORDER BY
    ""EquipmentId"", 
    ""RecipeName""", equipmentId);
            var data = sqlSugarClient.SqlQueryable<MachineRecipeCycleTime>(sql).ToList();
            if (data.Count > 0)
            {
                sqlSugarClient.Updateable<HandlerEquipmentStatus>()
                    .SetColumns(it => new HandlerEquipmentStatus()
                    {
                        InputTrayCT = data[0].InputTrayCT > 120 ? data[0].InputTrayCT : 120,
                        OutputTrayCT = data[0].OutputTrayCT > 120 ? data[0].OutputTrayCT : 120
                    })
                    .Where(it => it.Id == equipmentId)
                    .ExecuteCommand();
                dbgLog.Info($"{equipmentId}当前程式{data[0].Recipe}更新了LoaderCT和UnloaderCT为{data[0].InputTrayCT}和{data[0].OutputTrayCT}");
            }
            else
            {
                sqlSugarClient.Updateable<HandlerEquipmentStatus>()
                       .SetColumns(it => new HandlerEquipmentStatus()
                       {
                           InputTrayCT = 120,
                           OutputTrayCT = 120
                       })
                       .Where(it => it.Id == equipmentId)
                       .ExecuteCommand();
                dbgLog.Info($"{equipmentId}未找到当前程式记录，更新默认LoaderCT和UnloaderCT为120");
            }
        }

        public void UpdateAllCycleTime()
        {
            string sql = string.Format(@"WITH EventWithLag AS (
    SELECT 
        EH.""EquipmentId"",
        EH.""EventTime"",
        EH.""RecipeName"",
        EH.""EventName"",
        LAG(""EventTime"") OVER (PARTITION BY EH.""EquipmentId"", EH.""RecipeName"", EH.""EventName"" ORDER BY EH.""EventTime"") AS ""PreviousEventTime""
    FROM 
        ""HandlerEventHist"" EH, ""HandlerEquipmentStatus"" ES
    WHERE 
        ""EventTime"" > SYSDATE - 7
        AND ""EventName"" IN ('LoadTrayIn', 'UnloadTrayOut')  -- 只考虑这两个事件
        AND ES.""Id"" = EH.""EquipmentId""
        AND ES.""RecipeName"" = EH.""RecipeName""    --当前设备的当前recipe
        --AND ES.""Id"" = 'H087'
    ORDER BY ""EventTime"" DESC
),
TimeDifferences AS (
    SELECT 
        ""EquipmentId"",
        ""RecipeName"",
        ""EventName"",
        ""EventTime"",
        ""PreviousEventTime"",
        ((""EventTime"" - ""PreviousEventTime"") * 86400) AS ""TimeDifference""  -- 转换为秒
    FROM 
        EventWithLag
    WHERE 
        ""PreviousEventTime"" IS NOT NULL
)
--SELECT * 
--FROM TimeDifferences
SELECT 
    ""EquipmentId"" AS ""Id"",
    ""RecipeName"" AS ""Recipe"",
    ROUND(PERCENTILE_CONT(0.3) WITHIN GROUP (ORDER BY CASE WHEN ""EventName"" = 'LoadTrayIn' THEN ""TimeDifference"" END), 0) AS ""InputTrayCT"",  
    ROUND(PERCENTILE_CONT(0.3) WITHIN GROUP (ORDER BY CASE WHEN ""EventName"" = 'UnloadTrayOut' THEN ""TimeDifference"" END), 0) AS ""OutputTrayCT""  
FROM 
    TimeDifferences
GROUP BY 
    ""EquipmentId"", 
    ""RecipeName""
ORDER BY
    ""EquipmentId"", 
    ""RecipeName""");
            var data = sqlSugarClient.SqlQueryable<HandlerEquipmentStatus>(sql).ToList();
            sqlSugarClient.Updateable<HandlerEquipmentStatus>(data).UpdateColumns(it => new { it.InputTrayCT, it.OutputTrayCT }).ExecuteCommand();
        }

        public List<HandlerEquipmentStatusVm> GetEquipmentVmData(IEnumerable<HandlerEquipmentStatus> data)
        {

            var vmData = mapper.Map<List<HandlerEquipmentStatusVm>>(data);
            vmData.ForEach(it =>
            {
                it.LoadEstimatedTime = it.InputTrayUpdateTime.AddSeconds(it.InputTrayNumber * it.InputTrayCT);
                it.UnloadEstimatedTime = it.OutputTrayUpdateTime.AddSeconds((18 - it.OutputTrayNumber) * it.OutputTrayCT);
            });
            return vmData;

        }


    }
}
