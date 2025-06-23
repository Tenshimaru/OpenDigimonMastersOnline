
namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class ExtraEvolutionMaterialAssetDTO
    {
        public long Id { get; set; }
        public int ItemId { get; set; }
        public int Amount { get; set; }

        public long ExtraEvolutionId { get; set; }
        public ExtraEvolutionAssetDTO ExtraEvolution { get; set; }
    }
}
