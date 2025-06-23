namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class MapRegionAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Region index.
        /// </summary>
        public byte Index { get; set; }

        /// <summary>
        /// Axys X position. 
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Axys Y position.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Region name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long MapRegionListId { get; set; }
        public MapRegionListAssetDTO MapRegionList { get; set; }
    }
}