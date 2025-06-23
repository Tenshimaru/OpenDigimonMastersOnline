using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AchievementAssetsQueryHandler : IRequestHandler<AchievementAssetsQuery, List<AchievementAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public AchievementAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AchievementAssetDTO>> Handle(AchievementAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAchievementAssetsAsync();
        }
    }
}