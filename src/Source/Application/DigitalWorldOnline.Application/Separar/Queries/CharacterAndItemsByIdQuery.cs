using MediatR;
using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CharacterAndItemsByIdQuery : IRequest<CharacterDTO?>
    {
        public long CharacterId { get; set; }

        public CharacterAndItemsByIdQuery(long characterId)
        {
            CharacterId = characterId;
        }
    }
}

