using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HandlerAgv.Service.Models.ViewModel
{
    public class AgvStatus
    {
        public List<AgvInfo> vehicles { get; set; }
    }
    public class AgvInfo
    {
        public int id { get; set; }
        public string nickname { get; set; }
        public int battery { get; set; }
        /// <summary>
        /// CHARGING
        /// ERROR
        /// UNKNOWN
        /// IDLE
        /// EXECUTING
        /// </summary>
        public string sys_state { get; set; }
    }
}
