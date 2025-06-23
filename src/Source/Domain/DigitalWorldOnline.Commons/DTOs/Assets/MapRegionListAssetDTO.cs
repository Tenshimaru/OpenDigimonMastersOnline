namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class MapRegionListAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client map id reference.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// Map name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Available map regions
        /// </summary>
        public List<MapRegionAssetDTO> Regions { get; set; }
    }
}