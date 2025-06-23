using DigitalWorldOnline.Commons.Enums.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterStateCommand : IRequest
    {
        public long CharacterId { get; set; }

        public CharacterStateEnum State { get; set; }

        public UpdateCharacterStateCommand(long characterId, CharacterStateEnum state)
        {
            CharacterId = characterId;
            State = state;
        }
    }
}