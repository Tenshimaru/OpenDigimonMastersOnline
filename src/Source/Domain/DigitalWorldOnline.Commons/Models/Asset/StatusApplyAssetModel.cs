namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class StatusApplyAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference value to the target evolution.
        /// </summary>
        public int StageValue { get; private set; }

        /// <summary>
        /// Appliable multiplier.
        /// </summary>
        public int ApplyValue { get; private set; }
    }
}