using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Models
{
    public class CathodeConfig
    {
        public List<CathodeSetting> CathodeSettings { get; set; } = new List<CathodeSetting>();
    }

    public class CathodeSetting
    {
        public int Seq { get; set; }
        public string CathodeId { get; set; }
    }
}
