

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed partial class ArenaRankingDailyItemRewardModel
    {
        public long Id { get; set; }
        public int ItemId { get; set; }
        public int Amount { get; set; }
        public byte RequiredCoins { get; set; }

       
    }
}
