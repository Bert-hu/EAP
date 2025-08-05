namespace HandlerAgv.Service.Models.ViewModel
{
    public class GetEquipmentStateRequest
    {
        public string TaskId { get; set; }
        public string TaskType { get; set; }
        public string AgvId { get; set; }
    }

    public class TaskFeedBackRequest
    {
        public string TaskId { get; set; }
        public string Result { get; set; }
    }
}
