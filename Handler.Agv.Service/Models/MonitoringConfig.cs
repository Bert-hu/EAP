namespace HandlerAgv.Service.Models
{
    public class MonitoringConfig
    {
        //上限
        public double UpperLimit { get; set; } = 35.0;
        //下限
        public double LowerLimit { get; set; } = 33.0;
        //通知的邮箱List
        public List<string> EmailList { get; set; } = new List<string>();

        public List<string> PhoneList { get; set; } = new List<string>();
        //EQID最新监控时间
        public Dictionary<string, DateTime> EquipmentMonitorTime { get; set; } = new Dictionary<string, DateTime>() { };

        public Dictionary<string, SpecificConfig> EquipmentConfig { get; set; } = new Dictionary<string, SpecificConfig>() { };
    }

    public class SpecificConfig
    {
        public double UpperLimit { get; set; } = 35.0;
        //下限
        public double LowerLimit { get; set; } = 33.0;

    }
}
