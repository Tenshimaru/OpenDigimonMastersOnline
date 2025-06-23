using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class MapMobConfigsQueryHandler : IRequestHandler<MapMobConfigsQuery, IList<MobConfigDTO>>
    {
        private readonly IConfigQueriesRepository _repository;

        public MapMobConfigsQueryHandler(IConfigQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<MobConfigDTO>> Handle(MapMobConfigsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetMapMobsConfigAsync(request.MapConfigId);
        }
    }
}