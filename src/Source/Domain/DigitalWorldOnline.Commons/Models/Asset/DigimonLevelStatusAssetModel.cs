namespace DigitalWorldOnline.Commons.Models.Asset
{
    public class DigimonLevelStatusAssetModel : StatusAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference digimon type for the atributes.
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// Level reference for the status.
        /// </summary>
        public byte Level { get; private set; }

        /// <summary>
        /// Exp required for the next level.
        /// </summary>
        public long ExpValue { get; private set; }

        /// <summary>
        /// Status unique id.
        /// </summary>
        public int StatusId { get; private set; }
        
        /// <summary>
        /// Scale type.
        /// </summary>
        public int ScaleType { get; private set; }
    }
}