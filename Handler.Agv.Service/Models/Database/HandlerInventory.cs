namespace HandlerAgv.Service.Models.Database
{
    public class HandlerInventory
    {
        public string MaterialName { get; set; }
        public string GroupName { get; set; }
        public int EnableMachineCount { get; set; } = 0;
        public int AgvQuantity { get; set; } = 0;
        public int Stocker1Quantity { get; set; } = 0;
        public int Stocker2Quantity { get; set; } = 0;
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}
