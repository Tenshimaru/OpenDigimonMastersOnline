using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GameMapsConfigQueryHandler : IRequestHandler<GameMapsConfigQuery, List<MapConfigDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public GameMapsConfigQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MapConfigDTO>> Handle(GameMapsConfigQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetGameMapsConfigAsync(request.Type);
        }
    }
}