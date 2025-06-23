using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GameMapConfigByIdQueryHandler : IRequestHandler<GameMapConfigByIdQuery, MapConfigDTO?>
    {
        private readonly IConfigQueriesRepository _repository;

        public GameMapConfigByIdQueryHandler(IConfigQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<MapConfigDTO?> Handle(GameMapConfigByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetGameMapConfigByIdAsync(request.Id);
        }
    }
}