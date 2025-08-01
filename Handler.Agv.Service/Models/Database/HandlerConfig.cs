using SqlSugar;

namespace HandlerAgv.Service.Models.Database
{
    public class HandlerConfig
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string KEY { get; set; }
        [SugarColumn(IsNullable = true)]
        public string? VALUE { get; set; }
        [SugarColumn(IsNullable = true)]
        public string? DEBUGVALUE { get; set; }
        public DateTime UPDATETIME { get; set; } = DateTime.Now;
        [SugarColumn(IsNullable = true)]
        public string DESCRIPTION { get; set; } = string.Empty;
    }
}
