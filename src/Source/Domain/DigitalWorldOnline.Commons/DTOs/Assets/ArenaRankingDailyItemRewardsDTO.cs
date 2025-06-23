

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class ArenaRankingDailyItemRewardsDTO
    {
        public long Id { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public List<ArenaRankingDailyItemRewardDTO> Rewards { get; set; }
    }
}
