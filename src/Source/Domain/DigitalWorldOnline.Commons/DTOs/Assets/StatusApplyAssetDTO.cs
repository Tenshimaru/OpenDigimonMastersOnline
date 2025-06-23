namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class StatusApplyAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference value to the target evolution.
        /// </summary>
        public int StageValue { get; set; }

        /// <summary>
        /// Appliable multiplier.
        /// </summary>
        public int ApplyValue { get; set; }
    }
}