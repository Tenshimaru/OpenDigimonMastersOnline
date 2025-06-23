namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public sealed class KillSpawnConfigDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }
    
        public List<KillSpawnSourceMobConfigDTO> SourceMobs { get; set; }
        public List<KillSpawnTargetMobConfigDTO> TargetMobs { get; set; }

        /// <summary>
        /// Flag for sending the packet to show the spawn on the minimap.
        /// </summary>
        public bool ShowOnMinimap { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long GameMapConfigId { get; set; }
        public MapConfigDTO GameMapConfig { get; set; }
    }
}
