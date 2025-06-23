using DigitalWorldOnline.Commons.DTOs.Mechanics;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GuildByGuildNameQueryHandler : IRequestHandler<GuildByGuildNameQuery, GuildDTO?>
    {
        private readonly IServerQueriesRepository _repository;

        public GuildByGuildNameQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GuildDTO?> Handle(GuildByGuildNameQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetGuildByGuildNameAsync(request.GuildName);
        }
    }
}