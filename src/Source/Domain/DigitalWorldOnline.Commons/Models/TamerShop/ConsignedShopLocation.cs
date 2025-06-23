namespace DigitalWorldOnline.Commons.Models.TamerShop
{
    public class ConsignedShopLocation : Location
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        public static ConsignedShopLocation Create(short mapId, int x, int y)
        {
            var location = new ConsignedShopLocation();
            location.SetX(x);
            location.SetY(y);
            location.SetMapId(mapId);

            return location;
        }
    }
}
