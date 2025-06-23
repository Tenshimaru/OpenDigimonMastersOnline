using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateCharacterCommand : IRequest<long>
    {
        public CharacterModel Character { get; set; }

        public CreateCharacterCommand(CharacterModel character)
        {
            Character = character;
        }
    }
}
