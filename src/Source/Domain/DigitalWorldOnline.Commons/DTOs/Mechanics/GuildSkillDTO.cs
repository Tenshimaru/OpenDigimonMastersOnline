namespace DigitalWorldOnline.Commons.DTOs.Mechanics
{
    public class GuildSkillDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Skill code id.
        /// </summary>
        public int SkillId { get; set; }

        /// <summary>
        /// Guild skill current level.
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public GuildDTO Guild { get; set; }
        public long GuildId { get; set; }
    }
}