using DigitalWorldOnline.Commons.Enums.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Delete
{
    public class DeleteCharacterCommand : IRequest<DeleteCharacterResultEnum>
    {
        public long AccountId { get; set; }

        public byte CharacterPosition { get; set; }

        public DeleteCharacterCommand(long accountId, byte characterPosition)
        {
            AccountId = accountId;
            CharacterPosition = characterPosition;
        }
    }
}
