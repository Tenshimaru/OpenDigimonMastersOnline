namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class MapRegionListAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Client map id reference.
        /// </summary>
        public int MapId { get; private set; }

        /// <summary>
        /// Map name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Available map regions
        /// </summary>
        public List<MapRegionAssetModel> Regions { get; private set; }
    }
}