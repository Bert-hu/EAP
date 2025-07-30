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
        [SugarColumn(IsNullable = true, ColumnDescription = "AGV送料机种")]
        public string? MaterialName { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "AGV送料站别")]
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
        public string IP { get; set; } = string.Empty;
        public int InputTrayCT { get; set; } = 120;
        public int OutputTrayCT { get; set; } = 120;
    }
}
