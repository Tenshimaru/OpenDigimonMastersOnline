using DigitalWorldOnline.Commons.Enums.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharactersStateCommand : IRequest
    {
        public CharacterStateEnum State { get; set; }

        public UpdateCharactersStateCommand(CharacterStateEnum state)
        {
            State = state;
        }
    }
}