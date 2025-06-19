using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Model.Database
{
    class MachineConfig
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string MachineId { get; set; }
        public DateTime LastUploadEventTime { get; set; } = DateTime.Now.AddDays(-15);
    }
}
