using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public sealed class MapConfigDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client id reference to target map.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// Map name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Map type enumeration.
        /// </summary>
        public MapTypeEnum Type { get; set; }

        /// <summary>
        /// Client id reference to target map.
        /// </summary>
        public int Channels { get; set; }

        public int MapRegionindex { get; set; }

        /// <summary>
        /// Child mobs.
        /// </summary>
        public List<MobConfigDTO> Mobs { get; set; }
        
        /// <summary>
        /// Kill spawn list.
        /// </summary>
        public List<KillSpawnConfigDTO> KillSpawns { get; set; }
    }
}
