namespace DigitalWorldOnline.Commons.DTOs.Character
{
    public class CharacterXaiDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the client's item id.
        /// </summary>
        public int ItemId { get; set; }
        
        /// <summary>
        /// Total XGauge.
        /// </summary>
        public int XGauge { get; set; }
        
        /// <summary>
        /// Total XCrystals.
        /// </summary>
        public short XCrystals { get; set; }
        
        /// <summary>
        /// Reference to the owner tamer.
        /// </summary>
        public long CharacterId { get; set; }
        public CharacterDTO Character { get; set; }
    }
}