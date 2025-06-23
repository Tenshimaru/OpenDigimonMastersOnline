


namespace DigitalWorldOnline.Commons.Models.Config
{
    public sealed partial class KillSpawnConfigModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        public List<KillSpawnSourceMobConfigModel> SourceMobs { get; set; }

        /// <summary>
        /// Target mob identifier.
        /// </summary>
        public List<KillSpawnTargetMobConfigModel> TargetMobs { get; private set; }

        /// <summary>
        /// Flag for sending the packet to show the spawn on the minimap.
        /// </summary>
        public bool ShowOnMinimap { get; private set; }

        public List<KillSpawnTargetMobConfigModel> TempMobs { get; private set; }

      
    }
}
