namespace HandlerAgv.Service.Models.Inventory
{
    [SqlSugar.SugarTable("USI_STOCK_A2F_HANDLE.HANDLE_LOT_LOCATION")]
    public class StockerInventory_II
    {
        public string MODELNAME { get; set; }
        public string GROUPNAME { get; set; }
        public string STATUS { get; set; }
    }
}
