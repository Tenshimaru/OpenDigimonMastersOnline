namespace DigitalWorldOnline.Commons.Models.Config
{
    public class FruitConfigModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Fruit target item identifier.
        /// </summary>
        public int ItemId { get; private set; }

        /// <summary>
        /// Fruit target item section.
        /// </summary>
        public int ItemSection { get; private set; }

        /// <summary>
        /// Available chances.
        /// </summary>
        public List<FruitSizeConfigModel> SizeList { get; private set; }
    }
}