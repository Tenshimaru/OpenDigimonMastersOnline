namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class AccessoryRollAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference to the target item.
        /// </summary>
        public int ItemId { get; private set; }

        /// <summary>
        /// Status amount.
        /// </summary>
        public int StatusAmount { get; private set; }

        /// <summary>
        /// Max possible status reroll amount.
        /// </summary>
        public int RerollAmount { get; private set; }

        /// <summary>
        /// Available status list.
        /// </summary>
        public List<AccessoryRollStatusAssetModel> Status { get; set; }
    }
}