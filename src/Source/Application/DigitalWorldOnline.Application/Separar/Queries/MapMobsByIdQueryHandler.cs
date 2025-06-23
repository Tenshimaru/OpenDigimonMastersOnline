using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class MapMobsByIdQueryHandler : IRequestHandler<MapMobsByIdQuery, List<MobConfigDTO>>
    {
        private readonly IConfigQueriesRepository _repository;

        public MapMobsByIdQueryHandler(IConfigQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MobConfigDTO>> Handle(MapMobsByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetMapMobsByIdAsync(request.MapId);
        }
    }
}