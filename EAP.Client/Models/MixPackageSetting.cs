using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Models
{
    public class MixPackageSetting
    {
        public Dictionary<string, CountSetting> Settings { get; set; } = new Dictionary<string, CountSetting>();
    }

    public class CountSetting
    {
        public int IcosCount { get; set; } = 0;
        public int MCount { get; set; } = 0;
        public int OhCount { get; set; } = 0;
    }
}
