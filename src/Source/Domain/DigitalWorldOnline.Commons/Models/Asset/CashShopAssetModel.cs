namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class CashShopAssetModel
    {
        public int Id { get; set; }
        public int Item_Id { get; set; }
        public int Unique_Id { get; set; }
        public int Quanty { get; set; }
        public int Price { get; set; }
        public int Activated { get; set; }
        public string ItemName { get; set; }
    }
}
