using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class MapRegionListAssetsByMapIdQueryHandler : IRequestHandler<MapRegionListAssetsByMapIdQuery, MapRegionListAssetDTO?>
    {
        private readonly IServerQueriesRepository _repository;

        public MapRegionListAssetsByMapIdQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<MapRegionListAssetDTO?> Handle(MapRegionListAssetsByMapIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetMapRegionListAssetsAsync(request.MapId);
        }
    }
}