namespace DigitalWorldOnline.Commons.Models.Config
{
    public class MobExpRewardConfigModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Tamer base exp reward.
        /// </summary>
        public long TamerExperience { get; private set; }

        /// <summary>
        /// Digimon base exp reward.
        /// </summary>
        public long DigimonExperience { get; private set; }

        /// <summary>
        /// Digimon nature attribute base exp reward.
        /// </summary>
        public short NatureExperience { get; private set; }

        /// <summary>
        /// Digimon element attribute base exp reward.
        /// </summary>
        public short ElementExperience { get; private set; }

        /// <summary>
        /// Digimon skill experience base reward.
        /// </summary>
        public short SkillExperience { get; private set; }
    }
}
