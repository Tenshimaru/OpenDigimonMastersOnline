using MediatR;
using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CharacterByIdQuery : IRequest<CharacterDTO?>
    {
        public long CharacterId { get; set; }

        public CharacterByIdQuery(long characterId)
        {
            CharacterId = characterId;
        }
    }
}

