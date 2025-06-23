namespace DigitalWorldOnline.Commons.DTOs.Assets
{

    public class ExtraEvolutionInformationAssetDTO
    {
        public long Id { get; set; }

        public int IndexId { get; set; }
  
        public List<ExtraEvolutionAssetDTO> ExtraEvolution { get; set; }

        public long ExtraNpcId { get; set; }
        public ExtraEvolutionNpcAssetDTO ExtraNpcAsset { get; set; }
    }
}
