namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonDigicloneModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference to the digimon owner of this clone.
        /// </summary>
        public long DigimonId { get; private set; }

        /// <summary>
        /// Total AT clone level.
        /// </summary>
        public byte ATLevel { get; private set; }

        /// <summary>
        /// Total AT clone value.
        /// </summary>
        public short ATValue { get; private set; }

        /// <summary>
        /// Total BL clone level.
        /// </summary>
        public byte BLLevel { get; private set; }

        /// <summary>
        /// Total BL clone value.
        /// </summary>
        public short BLValue { get; private set; }

        /// <summary>
        /// Total CT clone level.
        /// </summary>
        public byte CTLevel { get; private set; }

        /// <summary>
        /// Total CT clone value.
        /// </summary>
        public short CTValue { get; private set; }

        /// <summary>
        /// Total EV clone level.
        /// </summary>
        public byte EVLevel { get; private set; }

        /// <summary>
        /// Total EV clone value.
        /// </summary>
        public short EVValue { get; private set; }

        /// <summary>
        /// Total HP clone level.
        /// </summary>
        public byte HPLevel { get; private set; }

        /// <summary>
        /// Total HP clone value.
        /// </summary>
        public short HPValue { get; private set; }

        public DigimonDigicloneHistoryModel History { get; private set; }

        public DigimonDigicloneModel()
        {
            History = new DigimonDigicloneHistoryModel();
        }
    }
}
