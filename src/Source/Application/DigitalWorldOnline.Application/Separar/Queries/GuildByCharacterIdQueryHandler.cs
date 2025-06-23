using DigitalWorldOnline.Commons.DTOs.Mechanics;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GuildByCharacterIdQueryHandler : IRequestHandler<GuildByCharacterIdQuery, GuildDTO?>
    {
        private readonly IServerQueriesRepository _repository;

        public GuildByCharacterIdQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GuildDTO?> Handle(GuildByCharacterIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetGuildByCharacterIdAsync(request.CharacterId);
        }
    }
}