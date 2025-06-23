using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class MapAssetDTO
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
        /// Map name.
        /// </summary>
        public MapTypeEnum Type { get; set; }

        /// <summary>
        /// Map region index.
        /// </summary>
        public byte RegionIndex { get; set; }
    }
}