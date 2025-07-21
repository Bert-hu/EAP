using SqlSugar;
namespace LaserMonitor.Service.Models
{
    [SugarTable("EQUIPMENTPARAMSHISRAW")]
    public class EquipmentParamsHisRaw
    {
        public string EQID { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
        public DateTime UPDATETIME { get; set; }
    }
}
