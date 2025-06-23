namespace DigitalWorldOnline.Commons.DTOs.Digimon
{
    public class DigimonDigicloneDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Total AT clone level.
        /// </summary>
        public byte ATLevel { get; set; }

        /// <summary>
        /// Total AT clone value (1 = 1,107).
        /// </summary>
        public short ATValue { get; set; }

        /// <summary>
        /// Total BL clone level.
        /// </summary>
        public byte BLLevel { get; set; }

        /// <summary>
        /// Total BL clone value (1 = 1%).
        /// </summary>
        public short BLValue { get; set; }

        /// <summary>
        /// Total CT clone level.
        /// </summary>
        public byte CTLevel { get; set; }

        /// <summary>
        /// Total CT clone value (1 = 0,1756%).
        /// </summary>
        public short CTValue{ get; set; }

        /// <summary>
        /// Total EV clone level.
        /// </summary>
        public byte EVLevel { get; set; }

        /// <summary>
        /// Total EV clone value (1 = 0,266%).
        /// </summary>
        public short EVValue { get; set; }

        /// <summary>
        /// Total HP clone level.
        /// </summary>
        public byte HPLevel { get; set; }

        /// <summary>
        /// Total HP clone value (1 = 8,928).
        /// </summary>
        public short HPValue { get; set; }
        
        public DigimonDigicloneHistoryDTO History { get; set; }

        /// <summary>
        /// Reference to the digimon owner of this clone.
        /// </summary>
        public long DigimonId { get; set; }
        public DigimonDTO Digimon { get; set; }
    }
}
