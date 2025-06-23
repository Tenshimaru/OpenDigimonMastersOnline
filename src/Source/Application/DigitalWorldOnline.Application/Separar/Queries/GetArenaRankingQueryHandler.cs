using DigitalWorldOnline.Commons.DTOs.Events;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Mechanics;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetArenaRankingQueryHandler : IRequestHandler<GetArenaRankingQuery, ArenaRankingDTO>
    {
        private readonly IServerQueriesRepository _repository;

        public GetArenaRankingQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<ArenaRankingDTO> Handle(GetArenaRankingQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetArenaRankingAsync(request.Ranking);
        }
    }
}