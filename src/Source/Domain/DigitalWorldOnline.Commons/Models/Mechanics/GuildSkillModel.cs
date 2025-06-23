namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public class GuildSkillModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Skill code id.
        /// </summary>
        public int SkillId { get; private set; }

        /// <summary>
        /// Guild skill current level.
        /// </summary>
        public byte Level { get; private set; }
    }
}
