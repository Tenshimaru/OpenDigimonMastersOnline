namespace DigitalWorldOnline.Commons.DTOs.Mechanics
{
    public class GuildDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Guild creation date.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Guild name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Guild level.
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// Guild current experience.
        /// </summary>
        public int CurrentExperience { get; set; }

        /// <summary>
        /// Guild current notice.
        /// </summary>
        public string Notice { get; set; }

        /// <summary>
        /// Guild extra members slots.
        /// </summary>
        public byte ExtraSlots { get; set; }

        /// <summary>
        /// Guild member list.
        /// </summary>
        public List<GuildMemberDTO> Members { get; set; }

        /// <summary>
        /// Guild skill list.
        /// </summary>
        public List<GuildSkillDTO> Skills { get; set; }

        /// <summary>
        /// Guild authority list.
        /// </summary>
        public List<GuildAuthorityDTO> Authority { get; set; }
        
        /// <summary>
        /// Guild historic list.
        /// </summary>
        public List<GuildHistoricDTO> Historic { get; set; }
    }
}