namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonEvolutionSkillModel
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

        public DigimonEvolutionSkillModel()
        {
            CurrentLevel = 1;
            MaxLevel = 10; //Até o 25 já na 487?
        }
    }
}
