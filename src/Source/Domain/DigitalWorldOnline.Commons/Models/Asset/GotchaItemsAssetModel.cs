namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class GotchaItemsAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ItemId
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// ItemId
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// Item name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ItemId
        /// </summary>
        public int InitialQuanty { get; set; }

        /// <summary>
        /// ItemId
        /// </summary>
        public int Quanty { get; set; }

        /// <summary>
        /// Parent object.
        /// </summary>
        public int GotchaId { get; set; }
        public GotchaAssetModel Gotcha { get; set; }
    }
}