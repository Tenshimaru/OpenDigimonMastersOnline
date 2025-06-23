namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class EvolutionArmorAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        public int ItemId { get; set; }
        public int Chance { get; set; }
        public int Amount { get; set; }

    }
}