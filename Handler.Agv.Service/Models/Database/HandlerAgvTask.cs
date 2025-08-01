using SqlSugar;
using SqlSugar.DbConvert;

namespace HandlerAgv.Service.Models.Database
{
    public enum AgvTaskType
    {
        Input,//开机仅上料
        InputOutput,//取paas料后上料
        Output//停机或超高仅取料
    }
    public enum AgvTaskStatus
    {
        AgvRequested = 1, // 已请求AGV
        AgvArrived = 2, // AGV已到达设备
        MachineReady = 3,//设备已锁定进出料,允许对接手臂
        AgvRobotFinished = 4, // AGV手臂任务完成
        Completed = 10,   // 已完成
        AbnormalEnd = 11  // 异常完结
    }
    public class HandlerAgvTask
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "子任务唯一码，对应请求的UPPERID")]
        public string ID { get; set; } = Guid.NewGuid().ToString("N");

        [SugarColumn(ColumnDataType = "VARCHAR2(50)", SqlParameterDbType = typeof(EnumToStringConvert), ColumnDescription = "任务类型：Input，Output，InputOutput")]
        public AgvTaskType Type { get; set; }
        [SugarColumn(ColumnDataType = "VARCHAR2(100)", SqlParameterDbType = typeof(EnumToStringConvert), ColumnDescription = "1-AgvRequested-已请求AGV， 2-AgvArrived-AGV已到达设备， 3-MachineLocked-设备已锁定进出料， 4-AgvRobotFinished-AGV手臂任务完成， 10-Completed-已完成， 11-AbnormalEnd-异常完结")]
        public AgvTaskStatus Status { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "设备EQID")]
        public string? EquipmentId { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "物料机种,用UPN前11码")]
        public string? MaterialId { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "物料站别")]
        public string? GroupName { get; set; }

        [SugarColumn(ColumnDescription = "AGV唯一码，AGV到位时更新")]
        public string AgvId { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "AGV请求时间")]
        public DateTime? AgvRequestTime { get; set; } = null;
        [SugarColumn(IsNullable = true, ColumnDescription = "AGV到达设备时间")]
        public DateTime? AgvArriveTime { get; set; } = null;
        [SugarColumn(IsNullable = true, ColumnDescription = "设备锁定进出料，允许对接手臂时间")]
        public DateTime? MachineReadyTime { get; set; } = null;

        [SugarColumn(IsNullable = true, ColumnDescription = "AGV手臂任务完成时间")]
        public DateTime? AgvRobotFinishedTime { get; set; } = null;

        [SugarColumn(IsNullable = true, ColumnDescription = "任务完成时间")]
        public DateTime? CompletedTime { get; set; } = null;
    }
}
