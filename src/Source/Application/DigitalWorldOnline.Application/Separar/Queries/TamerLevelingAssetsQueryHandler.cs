using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class TamerLevelingAssetsQueryHandler : IRequestHandler<TamerLevelingAssetsQuery, List<CharacterLevelStatusAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public TamerLevelingAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CharacterLevelStatusAssetDTO>> Handle(TamerLevelingAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetTamerLevelAssetsAsync();
        }
    }
}