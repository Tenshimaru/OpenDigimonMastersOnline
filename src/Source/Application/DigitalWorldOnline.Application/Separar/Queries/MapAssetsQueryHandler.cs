using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class MapAssetsQueryHandler : IRequestHandler<MapAssetsQuery, List<MapAssetDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public MapAssetsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MapAssetDTO>> Handle(MapAssetsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetMapAssetsAsync();
        }
    }
}