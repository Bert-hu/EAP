using SqlSugar;
namespace HandlerAgv.Service.Models.Database
{
    public class HandlerEquipmentStatus
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = string.Empty;
        public DateTime UpdateTime { get; set; } = DateTime.Now;
        public string ProcessState { get; set; } = string.Empty;
        public string ProcessStateCode { get; set; } = string.Empty;
        public string RecipeName { get; set; } = string.Empty;
        [SugarColumn(IsNullable = true, ColumnDescription = "AGV送料机种")]
        public string? MaterialName { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "AGV送料站别")]
        public string? GroupName { get; set; }
        [SugarColumn(ColumnDescription = "入料口Tray盘数")]
        public int InputTrayNumber { get; set; } = 0;
        [SugarColumn(ColumnDescription = "出料口Tray盘数")]
        public int OutputTrayNumber { get; set; } = 0;
        public bool AgvEnabled { get; set; } = false;
        public string IP { get; set; } = string.Empty;
        public int InputTrayCT { get; set; }
        public int OutputTrayCT { get; set; }
    }
}
