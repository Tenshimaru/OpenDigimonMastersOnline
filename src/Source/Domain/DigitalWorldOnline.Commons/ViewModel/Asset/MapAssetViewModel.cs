using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.ViewModel.Asset
{
    public class MapConfigViewModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client id reference to target map.
        /// </summary>
        public int MapId { get; private set; }

        /// <summary>
        /// Map name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Map type enumeration.
        /// </summary>
        public MapTypeEnum Type { get; private set; }

        /// <summary>
        /// Map region index.
        /// </summary>
        public byte RegionIndex { get; set; }
    }
}