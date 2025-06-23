using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CharactersByAccountIdQueryHandler : IRequestHandler<CharactersByAccountIdQuery, IList<CharacterDTO>>
    {
        private readonly ICharacterQueriesRepository _repository;

        public CharactersByAccountIdQueryHandler(ICharacterQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<CharacterDTO>> Handle(CharactersByAccountIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetCharactersByAccountIdAsync(request.AccountId);
        }
    }
}
