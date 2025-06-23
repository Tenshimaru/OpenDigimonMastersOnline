namespace DigitalWorldOnline.Commons.Models.TamerShop
{
    public sealed class ConsignedShop
    {
        public long Id { get; private set; }
        public string ShopName { get; private set; }
        public int ItemId { get; private set; }
        public int Channel { get; private set; }
        public long CharacterId { get; private set; }
        public uint GeneralHandler { get; private set; }
        public ConsignedShopLocation Location { get; private set; }

        public static ConsignedShop Create(long characterId, string shopName, int x, int y, int mapId, int channel, int itemId)
        {
            return new ConsignedShop()
            {
                CharacterId = characterId,
                ShopName = shopName,
                ItemId = itemId,
                Channel = channel,
                GeneralHandler = (uint)(114900 + characterId),
                Location = ConsignedShopLocation.Create((short)mapId, x, y)
            };
        }

        public void SetId(long id) => Id = id;
        public void SetGeneralHandler(int generalHandler) => GeneralHandler = (uint)generalHandler;
    }
}