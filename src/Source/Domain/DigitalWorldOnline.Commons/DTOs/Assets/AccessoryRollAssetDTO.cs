namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class AccessoryRollAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the target item.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Status amount.
        /// </summary>
        public int StatusAmount { get; set; }
        
        /// <summary>
        /// Max possible status reroll amount.
        /// </summary>
        public int RerollAmount { get; set; }
        
        /// <summary>
        /// Available status list.
        /// </summary>
        public List<AccessoryRollStatusAssetDTO> Status { get; set; }
    }
}