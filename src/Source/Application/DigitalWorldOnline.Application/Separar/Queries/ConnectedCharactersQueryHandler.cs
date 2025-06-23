using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class ConnectedCharactersQueryHandler : IRequestHandler<ConnectedCharactersQuery, IList<CharacterDTO>>
    {
        private readonly IAccountQueriesRepository _repository;

        public ConnectedCharactersQueryHandler(IAccountQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<CharacterDTO>> Handle(ConnectedCharactersQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetConnectedCharactersAsync();
        }
    }
}
