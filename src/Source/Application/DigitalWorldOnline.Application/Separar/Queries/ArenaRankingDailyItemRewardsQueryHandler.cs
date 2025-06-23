using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ArenaRankingDailyItemRewardsQueryHandler : IRequestHandler<ArenaRankingDailyItemRewardsQuery, List<ArenaRankingDailyItemRewardsDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public ArenaRankingDailyItemRewardsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ArenaRankingDailyItemRewardsDTO>> Handle(ArenaRankingDailyItemRewardsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetArenaRankingDailyItemRewardsAsync();
        }
    }
}