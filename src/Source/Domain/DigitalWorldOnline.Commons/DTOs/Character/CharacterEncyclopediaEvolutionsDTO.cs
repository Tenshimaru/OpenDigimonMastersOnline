using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterEncyclopediaEvolutionsDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Character encyclopedia identifier.
        /// </summary>
        public long CharacterEncyclopediaId { get; set; }

        /// <summary>
        /// Digimon base type.
        /// </summary>
        public int DigimonBaseType { get; set; }

        /// <summary>
        /// Required slot level to unlock.
        /// </summary>
        public byte SlotLevel { get; set; }

        /// <summary>
        /// Current display isUnlocked.
        /// </summary>
        public bool IsUnlocked { get; set; }

        /// <summary>
        /// Character creation date.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Encyclopedia.
        /// </summary>
        public CharacterEncyclopediaDTO? Encyclopedia { get; set; }

        /// <summary>
        /// Character base info.
        /// </summary>
        public DigimonBaseInfoAssetDTO? BaseInfo { get; set; }
    }
}