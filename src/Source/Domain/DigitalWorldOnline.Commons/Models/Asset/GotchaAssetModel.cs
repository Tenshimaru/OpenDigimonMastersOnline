namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class GotchaAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Npc Gotcha Id
        /// </summary>
        public int GotchaId { get; set; }

        /// <summary>
        /// Npc Gotcha Id
        /// </summary>
        public int NpcId { get; set; }

        /// <summary>
        /// Item Id of Necessary
        /// </summary>
        public int UseItem { get; set; }

        /// <summary>
        /// Item Quanty necessary
        /// </summary>
        public int UseCount { get; set; }

        /// <summary>
        /// Limit ???
        /// </summary>
        public short Limit { get; set; }

        /// <summary>
        /// Min lv to Usage
        /// </summary>
        public short MinLv { get; set; }

        /// <summary>
        /// Max Lv to usage
        /// </summary>
        public short MaxLv { get; set; }

        /// <summary>
        /// Quanty of Rare Items
        /// </summary>
        public short RareItemCnt { get; set; }

        /// <summary>
        /// Quanty of Rare Items
        /// </summary>
        public short Chance { get; set; }

        /// <summary>
        /// Normal Items
        /// </summary>
        public List<GotchaItemsAssetModel> Items { get; set; }

        /// <summary>
        /// Normal Items
        /// </summary>
        public List<GotchaRareItemsAssetModel> RareItems { get; set; }
    }
}