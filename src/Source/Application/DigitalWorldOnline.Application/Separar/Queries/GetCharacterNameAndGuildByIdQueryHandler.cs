using DigitalWorldOnline.Commons.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetCharacterNameAndGuildByIdQueryHandler : IRequestHandler<GetCharacterNameAndGuildByIdQuery, (string TamerName, string GuildName)>
    {
        private readonly ICharacterQueriesRepository _repository;

        public GetCharacterNameAndGuildByIdQueryHandler(ICharacterQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<(string TamerName, string GuildName)> Handle(GetCharacterNameAndGuildByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetCharacterNameAndGuildByIdQAsync(request.CharacterId);
        }
    }
}
