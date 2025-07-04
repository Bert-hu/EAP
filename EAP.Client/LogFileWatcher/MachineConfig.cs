using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.LogFileWatcher
{
    public class MachineConfig
    {
        //public string EquipmentId { get; set; } = string.Empty;
        public string Encoding { get; set; } = string.Empty;
        public string PathUserName { get; set; } = string.Empty;
        public string PathUserPassword { get; set; } = string.Empty;
        public List<string> FilePaths { get; set; } = new List<string>();
        public List<string> FileExtensions { get; set; } = new List<string>();
        public string TimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public string TimeRegex { get; set; } = "\\b\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}\\b";
        public Dictionary<string, string> StateDict { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> AlarmDict { get; set; } = new Dictionary<string, string>();
    }

}
