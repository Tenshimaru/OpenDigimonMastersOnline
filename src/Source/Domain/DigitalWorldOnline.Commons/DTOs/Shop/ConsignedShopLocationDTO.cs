using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Commons.DTOs.Shop
{
    public class ConsignedShopLocationDTO : LocationDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long ConsignedShopId { get; set; }
        public ConsignedShopDTO ConsignedShop { get; set; }
    }
}
