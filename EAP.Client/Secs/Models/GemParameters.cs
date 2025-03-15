using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Secs.Models
{
    public class GemCeid
    {
        public string Name { get; init; }
        public int ID { get; init; }
        public List<GemReport> Reports { get; init; }
    }
    public class GemReport
    {
        public int ID { get; init; }
        public List<GemSvid> Svids { get; init; }
    }
    public class GemSvid
    {
        public string Name { get; init; }
        public int ID { get; init; }
    }
}
