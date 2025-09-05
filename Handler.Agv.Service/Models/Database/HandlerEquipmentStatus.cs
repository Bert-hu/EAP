using SqlSugar;
namespace HandlerAgv.Service.Models.Database
{
    public class HandlerEquipmentStatus
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = string.Empty;

        [SugarColumn(IsNullable = true, ColumnDescription = "是否有效，用于判断排除测试设备")]
        public bool IsValiad { get; set; } = false;
        public DateTime UpdateTime { get; set; } = DateTime.Now;
        [SugarColumn(IsNullable = true, ColumnDescription = "Process状态")]
        public string? ProcessState { get; set; } = string.Empty;
        [SugarColumn(IsNullable = true, ColumnDescription = "Process状态码")]
        public string? ProcessStateCode { get; set; } = string.Empty;
        [SugarColumn(IsNullable = true, ColumnDescription = "Recipe名称")]
        public string? RecipeName { get; set; } = string.Empty;
        [SugarColumn(IsNullable = true, ColumnDescription = "当前报警Code List")]
        public string AlarmList { get; set; } = string.Empty;
        [SugarColumn(IsNullable = true, ColumnDescription = "设备是否处于清出状态，此状态不再上料")]
        public bool CleanOutState { get; set; } = false;
        [SugarColumn(IsNullable = true, ColumnDescription = "出料口是否已满盘")]
        public bool Auto1FullState { get; set; } = false;

        [SugarColumn(IsNullable = true, ColumnDescription = "物料机种,用UPN前11码")]
        public string? MaterialName { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "物料站别")]
        public string? GroupName { get; set; }
        [SugarColumn(ColumnDescription = "入料口Tray盘数")]
        public int InputTrayNumber { get; set; } = 0;
        [SugarColumn(ColumnDescription = "入料口Tray盘数更新时间",ColumnName = "InputTrayUpdateTime")]
        public DateTime InputTrayUpdateTime { get; set; } = DateTime.Now;
        [SugarColumn(ColumnDescription = "出料口Tray盘数")]
        public int OutputTrayNumber { get; set; } = 0;
        [SugarColumn(ColumnDescription = "出料口Tray盘数更新时间", ColumnName = "OutputTrayUpdateTime")]
        public DateTime OutputTrayUpdateTime { get; set; } = DateTime.Now;
        public bool AgvEnabled { get; set; } = false;
        [SugarColumn(IsNullable = true, ColumnDescription = "当前任务Id")]
        public string? CurrentTaskId { get; set; } = null;
        [SugarColumn(IsNullable = true, ColumnDescription = "当前Lot号")]
        public string? CurrentLot{ get; set; } = null;
        public string IP { get; set; } = string.Empty;
        public int InputTrayCT { get; set; } = 120;
        public int OutputTrayCT { get; set; } = 120;
        public bool LoaderEmpty { get; set; } = false;
    }
}
