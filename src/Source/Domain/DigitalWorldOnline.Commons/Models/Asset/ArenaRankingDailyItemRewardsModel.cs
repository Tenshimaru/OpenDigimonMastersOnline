

using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Commons.Models.Assets
{
    public sealed class ArenaRankingDailyItemRewardsModel
    { 
        public long Id { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public List<ArenaRankingDailyItemRewardModel> Rewards { get; set; }
        
        public List<ArenaRankingDailyItemRewardModel> GetRewards(int previousPoints,int currentPoints)
        {
             return Rewards.Where(x => previousPoints < x.RequiredCoins && currentPoints >= x.RequiredCoins)?.ToList();
        }
    }
}
