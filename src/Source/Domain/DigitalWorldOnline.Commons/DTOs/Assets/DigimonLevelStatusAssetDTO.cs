using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class DigimonLevelStatusAssetDTO : StatusDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference digimon type for the atributes.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Level reference for the status.
        /// </summary>
        public byte Level { get; set; }
        
        /// <summary>
        /// Exp required for the next level.
        /// </summary>
        public long ExpValue { get; set; }

        /// <summary>
        /// Status unique id.
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// Scale type.
        /// </summary>
        public int ScaleType { get; set; }
    }
}