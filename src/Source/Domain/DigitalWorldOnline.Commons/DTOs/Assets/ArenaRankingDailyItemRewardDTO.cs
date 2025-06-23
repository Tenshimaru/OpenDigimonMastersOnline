

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class ArenaRankingDailyItemRewardDTO
    {
        public long Id { get; set; }
        public int ItemId { get; set; }
        public int Amount { get; set; }
        public byte RequiredCoins { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public ArenaRankingDailyItemRewardsDTO Reward { get; set; }
        public long RewardId { get; set; }
    }
}
