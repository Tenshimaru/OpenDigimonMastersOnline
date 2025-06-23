namespace DigitalWorldOnline.Commons.DTOs.Digimon
{
    public class DigimonEvolutionSkillDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Current skill level.
        /// </summary>
        public byte CurrentLevel { get; private set; }

        /// <summary>
        /// Current Skill Cooldown.
        /// </summary>
        public int Duration { get; private set; }

        /// <summary>
        /// Current Skill Cooldown End Time.
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Max skill level.
        /// </summary>
        public byte MaxLevel { get; private set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public DigimonEvolutionDTO Evolution { get; private set; }
        public long EvolutionId { get; private set; }
    }
}