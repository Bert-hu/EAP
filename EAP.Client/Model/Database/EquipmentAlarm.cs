using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EAP.Client.Model.Database
{
    public class EquipmentAlarm
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime EventTime { get; set; }
        public string EquipmentId { get; set; }
        public string AlarmCode { get; set; }
        public string AlarmText { get; set; }
        public bool Uploaded { get; set; } = false;
    }
}
