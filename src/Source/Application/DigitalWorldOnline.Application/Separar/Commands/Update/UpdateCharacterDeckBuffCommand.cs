using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Events;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterDeckBuffCommand : IRequest
    {
        public CharacterModel Character { get; set; }

        public UpdateCharacterDeckBuffCommand(CharacterModel character)
        {
            Character = character;
        }
    }
}
