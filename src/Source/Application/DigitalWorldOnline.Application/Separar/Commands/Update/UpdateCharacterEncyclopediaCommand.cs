using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Events;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterEncyclopediaCommand : IRequest
    {
        public CharacterEncyclopediaModel CharacterEncyclopedia { get; set; }

        public UpdateCharacterEncyclopediaCommand(CharacterEncyclopediaModel characterEncyclopedia)
        {
            CharacterEncyclopedia = characterEncyclopedia;
        }
    }
}
