namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class AchievementAssetModel
    {
        /// <summary>
        /// Unique sequential identifier
        /// </summary>
        public long Id { get; set; }

        public short QuestId { get; set; }
      
        public byte Type { get; set; }

        public int BuffId { get; set; }
    }
}