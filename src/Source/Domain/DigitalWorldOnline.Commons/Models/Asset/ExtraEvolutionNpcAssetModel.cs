namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class ExtraEvolutionNpcAssetModel
    {
        public long Id { get; set; }
        public int NpcId { get; set; }
        public List<ExtraEvolutionInformationAssetModel> ExtraEvolutionInformation { get; set; }

        
    }
}
