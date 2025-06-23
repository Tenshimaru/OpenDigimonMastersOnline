namespace DigitalWorldOnline.Commons.DTOs.Base
{
    public class BuffDTO
    {
        /// <summary>
        /// Client reference for the target buff.
        /// </summary>
        public int BuffId { get; set; }

        /// <summary>
        /// Total duration in seconds.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Item TypeN Value.
        /// </summary>
        public int TypeN { get; set; }

        public int Cooldown{ get; set; }

        /// <summary>
        /// Client reference for the target skill id.
        /// </summary>
        public int SkillId { get; set; }

        /// <summary>
        /// Time when the buff expires
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Time when the buff expires
        /// </summary>
        public DateTime CoolEndDate { get; set; }
    }
}
