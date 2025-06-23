

namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public sealed class KillSpawnTargetMobConfigDTO
    {
        public long Id { get; set; }
        public int TargetMobType { get; set; }

        public byte TargetMobAmount { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long KillSpawnId { get; set; }
        public KillSpawnConfigDTO KillSpawn { get; set; }

    }
}
