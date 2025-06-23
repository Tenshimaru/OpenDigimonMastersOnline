namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public sealed partial class CharacterActiveEvolutionDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// DS usage per second.
        /// </summary>
        public int DsPerSecond { get; set; }

        /// <summary>
        /// XGauge usage per second.
        /// </summary>
        public int XgPerSecond { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long CharacterId { get; set; }
        public CharacterDTO Character { get; set; }
    }
}