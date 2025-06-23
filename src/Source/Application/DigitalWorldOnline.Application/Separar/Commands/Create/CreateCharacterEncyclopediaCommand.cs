using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateCharacterEncyclopediaCommand : IRequest<CharacterEncyclopediaModel>
    {
        public CharacterEncyclopediaModel CharacterEncyclopedia { get; set; }

        public CreateCharacterEncyclopediaCommand(CharacterEncyclopediaModel characterEncyclopedia)
        {
            CharacterEncyclopedia = characterEncyclopedia;
        }
    }
}
