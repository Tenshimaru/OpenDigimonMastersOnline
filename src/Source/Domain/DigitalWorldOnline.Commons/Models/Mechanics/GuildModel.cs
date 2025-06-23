namespace DigitalWorldOnline.Commons.Models.Mechanics
{
    public sealed partial class GuildModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Guild creation date.
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// Guild name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Guild level.
        /// </summary>
        public byte Level { get; private set; }
        
        /// <summary>
        /// Guild current experience.
        /// </summary>
        public int CurrentExperience { get; private set; }

        /// <summary>
        /// Guild current notice.
        /// </summary>
        public string Notice { get; private set; }

        /// <summary>
        /// Guild extra members slots.
        /// </summary>
        public byte ExtraSlots { get; private set; }

        /// <summary>
        /// Guild member list.
        /// </summary>
        public List<GuildMemberModel> Members { get; private set; }

        /// <summary>
        /// Guild skill list.
        /// </summary>
        public List<GuildSkillModel> Skills { get; private set; }

        /// <summary>
        /// Guild authority list.
        /// </summary>
        public List<GuildAuthorityModel> Authority { get; private set; }
        
        /// <summary>
        /// Guild historic list.
        /// </summary>
        public List<GuildHistoricModel> Historic { get; private set; }
    }
}