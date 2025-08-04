namespace HandlerAgv.Service.Models.ViewModel
{
    public class AgvTaskRequest
    {
        public string TaskId { get; set; } = Guid.NewGuid().ToString();
        public string TaskType { get; set; }
        public string EQID { get; set; }
        public string MaterialName { get; set; }
        public string GroupName { get; set; }
    }
    public class AgvTaskResponse
    {
        public bool Result { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}
