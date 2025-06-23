
namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public class SummonMobLocationDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference ID to map.
        /// </summary>
        public short MapId { get; set; }

        /// <summary>
        /// Position X.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Position Y.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Position Z (looking for).
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long MobConfigId { get; set; }
        public SummonMobDTO MobConfig { get; set; }

    }
}
