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
    }
    public class SputtereConfig
    {
        public string EmpNo { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public List<CathodeSetting> CathodeSettings { get; set; } = new List<CathodeSetting>();
    }

    public class CathodeSetting
    {
        public int Seq { get; set; }
        public string CathodeId { get; set; }
    }
}
