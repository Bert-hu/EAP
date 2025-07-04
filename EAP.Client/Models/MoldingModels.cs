using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Models
{
    public class SnInfo
    {
        public string SN { get; set; }
        public string CarrierId { get; set; }
        public string ModelName { get; set; }
        public string ProjectName { get; set; }
    }
    public class MoldingConfig
    {
        public string EmpNo { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string ReelId { get; set; } = string.Empty;
        public bool AutoCheckRecipeName { get; set; } = true;
        public bool AutoCheckRecipeBody { get; set; } = true;
        public Dictionary<string, string> UserPassword { get; set; } = new Dictionary<string, string>() { };
    }

}
