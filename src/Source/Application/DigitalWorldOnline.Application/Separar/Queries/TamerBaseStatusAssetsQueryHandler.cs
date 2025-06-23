using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class TamerBaseStatusAssetsQueryHandler : IRequestHandler<TamerBaseStatusAssetsQuery, List<CharacterBaseStatusAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public TamerBaseStatusAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CharacterBaseStatusAssetDTO>> Handle(TamerBaseStatusAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllTamerBaseStatusAsync();
        }
    }
}