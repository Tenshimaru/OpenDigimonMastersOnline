namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public partial class DigimonDigicloneHistoryModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        public int[] ATValues { get; private set; }
        public int[] BLValues { get; private set; }
        public int[] CTValues { get; private set; }
        public int[] EVValues { get; private set; }
        public int[] HPValues { get; private set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long DigimonDigicloneId { get; private set; }

        public DigimonDigicloneHistoryModel()
        {
            ATValues = new int[15] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            BLValues = new int[15] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            CTValues = new int[15] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            EVValues = new int[15] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            HPValues = new int[15] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
        }
    }
}