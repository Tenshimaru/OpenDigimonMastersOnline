namespace DigitalWorldOnline.Commons.Models.Asset
{ 
    public class ExtraEvolutionAssetModel
    {
        public long Id { get; set; }

        public int DigimonId { get; set; }

        public byte RequiredLevel { get; set; }

        public long Price { get; set; }

        public List<ExtraEvolutionMaterialAssetModel> Materials { get; set; }

        public List<ExtraEvolutionRequiredAssetModel> Requireds { get; set; }
    }
}
