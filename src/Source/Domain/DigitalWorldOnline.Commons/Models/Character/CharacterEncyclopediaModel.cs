using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Base;

namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterEncyclopediaModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Character identifier.
        /// </summary>
        public long CharacterId { get; private set; }

        /// <summary>
        /// Evolution identifier.
        /// </summary>
        public long DigimonEvolutionId { get; private set; }

        public short Level { get; private set; }

        public short Size { get; private set; }

        public short EnchantAT { get; private set; }

        public short EnchantBL { get; private set; }

        public short EnchantCT { get; private set; }

        public short EnchantEV { get; private set; }

        public short EnchantHP { get; private set; }

        /// <summary>
        /// Current display IsRewardAllowed.
        /// </summary>
        public bool IsRewardAllowed { get; private set; }

        /// <summary>
        /// Current display IsRewardReceived.
        /// </summary>
        public bool IsRewardReceived { get; private set; }

        /// <summary>
        /// Character creation date.
        /// </summary>
        public DateTime CreateDate { get; private set; }

        /// <summary>
        /// Character.
        /// </summary>
        public CharacterModel Character { get; private set; }

        /// <summary>
        /// evolution assets.
        /// </summary>
        public EvolutionAssetModel EvolutionAsset { get; private set; }

        /// <summary>
        /// Character encyclopedia evolutions
        /// </summary>
        public List<CharacterEncyclopediaEvolutionsModel> Evolutions { get; private set; }

        public CharacterEncyclopediaModel()
        {
            Evolutions = new List<CharacterEncyclopediaEvolutionsModel>();
        }

        public long GetId()
        {
            return Id;
        }
    }
}