using DigitalWorldOnline.Commons.Models.Asset;

namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterEncyclopediaEvolutionsModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Character encyclopedia identifier.
        /// </summary>
        public long CharacterEncyclopediaId { get; private set; }

        /// <summary>
        /// Digimon base type.
        /// </summary>
        public int DigimonBaseType { get; private set; }

        /// <summary>
        /// Required slot level to unlock.
        /// </summary>
        public byte SlotLevel { get; private set; }

        /// <summary>
        /// Current display isUnlocked.
        /// </summary>
        public bool IsUnlocked { get; private set; }

        /// <summary>
        /// Character creation date.
        /// </summary>
        public DateTime CreateDate { get; private set; }

        /// <summary>
        /// Encyclopedia.
        /// </summary>
        public CharacterEncyclopediaModel? Encyclopedia { get; private set; }

        /// <summary>
        /// Character base info.
        /// </summary>
        public DigimonBaseInfoAssetModel? BaseInfo { get; private set; }
    }
}