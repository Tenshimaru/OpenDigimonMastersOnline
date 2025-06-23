using MediatR;
using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CharacterByAccountIdAndPositionQueryHandler : IRequestHandler<CharacterByAccountIdAndPositionQuery, CharacterDTO?>
    {
        private readonly ICharacterQueriesRepository _repository;

        public CharacterByAccountIdAndPositionQueryHandler(ICharacterQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<CharacterDTO?> Handle(CharacterByAccountIdAndPositionQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetCharacterByAccountIdAndPositionAsync(request.AccountId, request.Position);
        }
    }
}
