using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class GotchaRareItemsAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ItemId
        /// </summary>
        public int RareItem { get; set; }

        /// <summary>
        /// ItemId
        /// </summary>
        public short RareItemCnt { get; set; }

        /// <summary>
        /// Item name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ItemId
        /// </summary>
        public short RareItemGive { get; set; }

        /// <summary>
        /// Parent object.
        /// </summary>
        public int GotchaId { get; set; }
        public GotchaAssetDTO Gotcha { get; set; }
    }
}