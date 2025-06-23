using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CharacterByIdQueryHandler : IRequestHandler<CharacterByIdQuery, CharacterDTO?>
    {
        private readonly ICharacterQueriesRepository _repository;

        public CharacterByIdQueryHandler(ICharacterQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<CharacterDTO?> Handle(CharacterByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetCharacterByIdAsync(request.CharacterId);
        }
    }
}
