namespace HandlerAgv.Service.Models.Database
{
    public class HandlerEventHist
    {
        public DateTime EventTime { get; set; } = DateTime.Now;
        public string EquipmentId { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string ProcessState { get; set; } = string.Empty;
        public string RecipeName { get; set; } = string.Empty;
    }
}
