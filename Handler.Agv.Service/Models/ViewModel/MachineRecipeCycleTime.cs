namespace HandlerAgv.Service.Models.ViewModel
{
    public class MachineRecipeCycleTime
    {
        public string EquipmentId { get; set; }
        public string Recipe { get; set; }
        public int InputTrayCT { get; set; }
        public int OutputTrayCT { get; set; }
    }
}
