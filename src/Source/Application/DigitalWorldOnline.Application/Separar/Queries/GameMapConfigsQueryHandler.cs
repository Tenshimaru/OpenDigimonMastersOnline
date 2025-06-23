using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GameMapConfigsQueryHandler : IRequestHandler<GameMapConfigsQuery, IList<MapConfigDTO>>
    {
        private readonly IConfigQueriesRepository _repository;

        public GameMapConfigsQueryHandler(IConfigQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<MapConfigDTO>> Handle(GameMapConfigsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetGameMapConfigsAsync();
        }
    }
}