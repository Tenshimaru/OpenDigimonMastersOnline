using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterEncyclopediaDTO
    {
        public long Id { get; set; }

        public long CharacterId { get; set; }

        public long DigimonEvolutionId { get; set; }

        public short Level { get; set; }

        public short Size { get; set; }

        public short EnchantAT { get; set; }

        public short EnchantBL { get; set; }

        public short EnchantCT { get; set; }

        public short EnchantEV { get; set; }

        public short EnchantHP { get; set; }

        public bool IsRewardAllowed { get; set; }

        public bool IsRewardReceived { get; set; }

        public DateTime CreateDate { get; set; }

        public List<CharacterEncyclopediaEvolutionsDTO>? Evolutions { get; set; }

        public CharacterDTO? Character { get; set; }

        public EvolutionAssetDTO? EvolutionAsset { get; set; }
    }
}