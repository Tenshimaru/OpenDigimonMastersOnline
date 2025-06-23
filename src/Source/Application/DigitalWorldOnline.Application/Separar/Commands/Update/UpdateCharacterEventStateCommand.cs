using DigitalWorldOnline.Commons.Enums.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterEventStateCommand : IRequest
    {
        public long CharacterId { get; set; }

        public CharacterEventStateEnum State { get; set; }

        public UpdateCharacterEventStateCommand(long characterId, CharacterEventStateEnum state)
        {
            CharacterId = characterId;
            State = state;
        }
    }
}