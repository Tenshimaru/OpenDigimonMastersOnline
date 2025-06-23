namespace DigitalWorldOnline.Commons.DTOs.Assets
{

    public class ExtraEvolutionAssetDTO
    {
        public long Id { get; set; }

        public int DigimonId { get; set; }

        public byte RequiredLevel { get; set; }

        public long Price { get; set; }

        public List<ExtraEvolutionMaterialAssetDTO> Materials { get; set; }

        public List<ExtraEvolutionRequiredAssetDTO> Requireds { get; set; }

        public long ExtraInfoId { get; set; }
        public ExtraEvolutionInformationAssetDTO ExtraInformationAsset { get; set; }
    }
}
