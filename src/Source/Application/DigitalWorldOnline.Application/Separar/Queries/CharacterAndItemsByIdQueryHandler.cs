using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CharacterAndItemsByIdQueryHandler : IRequestHandler<CharacterAndItemsByIdQuery, CharacterDTO?>
    {
        private readonly ICharacterQueriesRepository _repository;

        public CharacterAndItemsByIdQueryHandler(ICharacterQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<CharacterDTO?> Handle(CharacterAndItemsByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetCharacterAndItemsByIdAsync(request.CharacterId);
        }
    }
}
