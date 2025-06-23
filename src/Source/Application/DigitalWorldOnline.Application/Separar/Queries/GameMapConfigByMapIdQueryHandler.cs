using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GameMapConfigByMapIdQueryHandler : IRequestHandler<GameMapConfigByMapIdQuery, MapConfigDTO?>
    {
        private readonly IConfigQueriesRepository _repository;

        public GameMapConfigByMapIdQueryHandler(IConfigQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<MapConfigDTO?> Handle(GameMapConfigByMapIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetGameMapConfigByMapIdAsync(request.MapId);
        }
    }
}