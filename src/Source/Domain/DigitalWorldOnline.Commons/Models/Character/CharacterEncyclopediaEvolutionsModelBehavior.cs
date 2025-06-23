namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterEncyclopediaEvolutionsModel
    {
        /// <summary>
        /// Creates a new encyclopedia evolution.
        /// </summary>
        /// <param name="characterEncyclopediaId">Encyclopedia id</param>
        /// <param name="digimonBaseType">Digimon base type</param>
        /// <param name="slotLevel">Slot level</param>
        /// <param name="isUnlocked">Is unlocked</param>
        public static CharacterEncyclopediaEvolutionsModel Create(long characterEncyclopediaId, int digimonBaseType, byte slotLevel = 0, bool isUnlocked = false)
        {
            return new CharacterEncyclopediaEvolutionsModel()
            {
                CharacterEncyclopediaId = characterEncyclopediaId,
                DigimonBaseType = digimonBaseType,
                SlotLevel = slotLevel,
                IsUnlocked = isUnlocked,
            };
        }
        /// <summary>
        /// Creates a new encyclopedia evolution.
        /// </summary>
        /// <param name="characterEncyclopediaId">Encyclopedia id</param>
        /// <param name="digimonBaseType">Digimon base type</param>
        /// <param name="slotLevel">Slot level</param>
        /// <param name="isUnlocked">Is unlocked</param>
        public static CharacterEncyclopediaEvolutionsModel Create(int digimonBaseType, byte slotLevel = 0, bool isUnlocked = false)
        {
            return new CharacterEncyclopediaEvolutionsModel()
            {
                DigimonBaseType = digimonBaseType,
                SlotLevel = slotLevel,
                IsUnlocked = isUnlocked,
            };
        }

        /// <summary>
        /// Unlocks the target evolution.
        /// </summary>
        public void Unlock(bool value = true) => IsUnlocked = value;
    }
}