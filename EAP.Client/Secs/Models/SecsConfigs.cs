using System.Collections.Generic;

namespace EAP.Client.Secs.Models
{
    public class SecsConfigs
    {
        public int HeartBeatInterval { get; set; } = 0;
        public bool EnableAllAlarm { get; set; } = false;
        public bool EnableDynamicEvent { get; set; } = false;
        public bool EnableAllEvent { get; set; } = false;
        public List<SVID> SVIDList { get; set; } = new List<SVID>();
        public List<Report> ReportList { get; set; } = new List<Report> { };
        public List<CEID> CEIDList { get; set; } = new List<CEID>();
        public Dictionary<string, string> ProcessStateCodes { get; set; } = new Dictionary<string, string>();
    }

    public class SVID
    {
        public string Name { get; init; }
        public int ID { get; init; }
    }
    public class Report
    {
        public int ID { get; init; }
        public string[] SvidNames { get; init; }
    }
    public class CEID
    {
        public string Name { get; init; }
        public int ID { get; init; }
        public List<int> ReportIds { get; init; }
    }
}
