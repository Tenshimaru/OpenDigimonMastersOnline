using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GuildCurrentRankByGuildIdQueryHandler : IRequestHandler<GuildCurrentRankByGuildIdQuery, short>
    {
        private readonly IServerQueriesRepository _repository;

        public GuildCurrentRankByGuildIdQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<short> Handle(GuildCurrentRankByGuildIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetGuildRankByGuildIdAsync(request.GuildId);
        }
    }
}