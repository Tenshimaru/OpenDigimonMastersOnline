namespace DigitalWorldOnline.Commons.ViewModel.Mobs
{
    public class MobExpRewardViewModel
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
        /// Digimon nature attribute base exp reward.
        /// </summary>
        public short NatureExperience { get; set; }

        /// <summary>
        /// Digimon element attribute base exp reward.
        /// </summary>
        public short ElementExperience { get; set; }

        /// <summary>
        /// Digimon skill experience base reward.
        /// </summary>
        public short SkillExperience { get; set; }
    }
}
