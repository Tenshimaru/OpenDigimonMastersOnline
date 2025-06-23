using MediatR;
using DigitalWorldOnline.Commons.DTOs.Character;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CharacterByNameQuery : IRequest<CharacterDTO?>
    {
        public string CharacterName { get; set; }

        public CharacterByNameQuery(string characterName)
        {
            CharacterName = characterName;
        }
    }
}

