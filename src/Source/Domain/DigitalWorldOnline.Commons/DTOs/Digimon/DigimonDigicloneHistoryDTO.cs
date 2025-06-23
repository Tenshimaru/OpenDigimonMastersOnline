namespace DigitalWorldOnline.Commons.DTOs.Digimon
{
    public class DigimonDigicloneHistoryDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        public int[] ATValues { get; set; }
        public int[] BLValues { get; set; }
        public int[] CTValues { get; set; }
        public int[] EVValues { get; set; }
        public int[] HPValues { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long DigimonDigicloneId { get; set; }
        public DigimonDigicloneDTO DigimonDigiclone { get; set; }
    }
}