namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class TimeRewardAssetDTO
    {
        public int Id { get; set; }
        public int CurrentReward { get; set; }
        public int ItemId { get; set; }
        public int ItemCount { get; set; }
        public int RewardIndex { get; set; }
    }
}