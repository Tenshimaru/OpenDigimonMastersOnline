using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterDigimonsOrderCommand : IRequest
    {
        public CharacterModel Character { get; set; }

        public UpdateCharacterDigimonsOrderCommand(CharacterModel character)
        {
            Character = character;
        }
    }
}