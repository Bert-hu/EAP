namespace HandlerAgv.Service.Models.Inventory
{

    public class InventoryDetails
    {
        public List<InvRecord> records { get; set; }
    }
    public class InvRecord
    {
        public string materialName { get; set; }
        public string groupName { get; set; }
        public string testState { get; set; }
        public string occupiedSate { get; set; }
    }
}
