using DigitalWorldOnline.Commons.DTOs.Base;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class SealDetailAssetDTO : StatusDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Client reference to the target seal.
        /// </summary>
        public int SealId { get; set; }

        /// <summary>
        /// Required seals to attain the target status.
        /// </summary>
        public short RequiredAmount { get; set; }

        /// <summary>
        /// Sequential seal id.
        /// </summary>
        public short SequentialId { get; set; }
    }
}