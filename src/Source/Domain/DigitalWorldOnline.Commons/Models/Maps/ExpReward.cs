namespace DigitalWorldOnline.Commons.Models.Maps
{
    public class ExpReward
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Tamer base exp reward.
        /// </summary>
        public long TamerExperience { get; set; }

        /// <summary>
        /// Digimon base exp reward.
        /// </summary>
        public long DigimonExperience { get; set; }

        /// <summary>
        /// Digimon family attribute base exp reward.
        /// </summary>
        public long FamilyExperience { get; set; }

        /// <summary>
        /// Digimon property attribute base exp reward.
        /// </summary>
        public long PropertyExperience { get; set; }

        /// <summary>
        /// Digimon skill experience base reward.
        /// </summary>
        public long SkillExperience { get; set; } //TODO: definir o formato dessa recompensa

        /// <summary>
        /// Reference to the target digimon.
        /// </summary>
        public long MobId { get; set; }
    }
}
