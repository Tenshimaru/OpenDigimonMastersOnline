namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public class FruitConfigDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Fruit target item identifier.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Fruit target item section.
        /// </summary>
        public int ItemSection { get; set; }

        /// <summary>
        /// Available chances.
        /// </summary>
        public List<FruitSizeConfigDTO> SizeList { get; set; }
    }
}