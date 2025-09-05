using HandlerAgv.Service.Models.Database;

namespace HandlerAgv.Service.Models.ViewModel
{
    public class HandlerEquipmentStatusVm: HandlerEquipmentStatus
    {
        public DateTime? LoadEstimatedTime { get; set; }
        public DateTime? UnloadEstimatedTime { get; set; }
        public string? CurrentTaskState { get; set; }
        public DateTime? CurrentTaskRequestTime { get; set; }
    }
}
