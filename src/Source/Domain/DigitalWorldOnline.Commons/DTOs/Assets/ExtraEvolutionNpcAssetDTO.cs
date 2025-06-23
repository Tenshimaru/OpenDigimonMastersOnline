namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class ExtraEvolutionNpcAssetDTO
    {
        public long Id { get; set; }
        public int NpcId { get; set; }
        public List<ExtraEvolutionInformationAssetDTO> ExtraEvolutionInformation { get; set; }  
    }
}
