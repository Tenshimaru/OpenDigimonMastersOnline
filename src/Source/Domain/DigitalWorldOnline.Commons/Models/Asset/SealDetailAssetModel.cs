namespace DigitalWorldOnline.Commons.Models.Asset
{
    public class SealDetailAssetModel : StatusAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client reference to the target seal.
        /// </summary>
        public int SealId { get; private set; }

        /// <summary>
        /// Required seals to attain the target status.
        /// </summary>
        public short RequiredAmount { get; private set; }

        /// <summary>
        /// Sequential seal id.
        /// </summary>
        public short SequentialId { get; private set; }
    }
}