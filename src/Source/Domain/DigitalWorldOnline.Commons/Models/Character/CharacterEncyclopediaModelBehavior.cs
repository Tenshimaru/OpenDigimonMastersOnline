namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterEncyclopediaModel
    {
        public DateTime TempUpdating { get; set; } = DateTime.Now;

        /// <summary>
        /// Creates a new seal.
        /// </summary>
        /// <param name="characterId">Character id.</param>
        /// <param name="digimonEvolutionId">Digimon evolution id.</param>
        /// <param name="level">Digimon evolution id.</param>
        /// <param name="size">Digimon evolution id.</param>
        /// <param name="enchantAT">Digimon evolution id.</param>
        /// <param name="enchantBL">Digimon evolution id.</param>
        /// <param name="enchantCT">Digimon evolution id.</param>
        /// <param name="enchantEV">Digimon evolution id.</param>
        /// <param name="enchantHP">Digimon evolution id.</param>
        /// <param name="isRewardAllowed">Is reward allowed.</param>
        /// <param name="isRewardReceived">Is reward received.</param>
        public static CharacterEncyclopediaModel Create(
            long characterId,
            long digimonEvolutionId,
            short level,
            short size,
            short enchantAT = 0,
            short enchantBL = 0,
            short enchantCT = 0,
            short enchantEV = 0,
            short enchantHP = 0,
            bool isRewardAllowed = false,
            bool isRewardReceived = false
            )
        {
            return new CharacterEncyclopediaModel()
            {
                CharacterId = characterId,
                DigimonEvolutionId = digimonEvolutionId,
                Level = level,
                Size = size,
                EnchantAT = enchantAT,
                EnchantBL = enchantBL,
                EnchantCT = enchantCT,
                EnchantEV = enchantEV,
                EnchantHP = enchantHP,
                IsRewardAllowed = isRewardAllowed,
                IsRewardReceived = isRewardReceived
            };
        }

        /// <summary>
        /// Set Encyclopedia reward as true.
        /// </summary>
        public void SetRewardAllowed(bool value = true) => IsRewardAllowed = value;

        /// <summary>
        /// Unlocks the target evolution.
        /// </summary>
        public void SetRewardReceived(bool value = true) => IsRewardReceived = value;
    }
}