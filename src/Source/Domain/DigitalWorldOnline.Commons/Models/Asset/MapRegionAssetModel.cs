namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class MapRegionAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Region index.
        /// </summary>
        public byte Index { get; private set; }

        /// <summary>
        /// Axys X position. 
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Axys Y position.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Region name.
        /// </summary>
        public string Name { get; private set; }
    }
}