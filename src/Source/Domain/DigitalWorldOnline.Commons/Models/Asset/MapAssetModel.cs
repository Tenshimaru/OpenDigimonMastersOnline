namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class MapAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client reference to the target map.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// Map name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Map region index.
        /// </summary>
        public byte RegionIndex { get; set; }

        /// <summary>
        /// Map resurrection.
        /// </summary>
        public int ResurrectionMapID { get; set; }

        /// <summary>
        /// Map X-Gauge consume.
        /// </summary>
        public int XgConsumeType { get; set; }

        /// <summary>
        /// Map BattleTag use.
        /// </summary>
        public int BattleTagUse { get; set; }
    }
}