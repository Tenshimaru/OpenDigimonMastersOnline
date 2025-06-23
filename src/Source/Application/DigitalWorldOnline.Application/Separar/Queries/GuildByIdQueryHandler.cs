using DigitalWorldOnline.Commons.DTOs.Mechanics;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GuildByIdQueryHandler : IRequestHandler<GuildByIdQuery, GuildDTO?>
    {
        private readonly IServerQueriesRepository _repository;

        public GuildByIdQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GuildDTO?> Handle(GuildByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetGuildByIdAsync(request.GuildId);
        }
    }
}