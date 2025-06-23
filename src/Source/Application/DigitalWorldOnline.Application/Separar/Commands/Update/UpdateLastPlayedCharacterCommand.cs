using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateLastPlayedCharacterCommand : IRequest
    {
        public long AccountId { get; set; }

        public long CharacterId { get; set; }

        public UpdateLastPlayedCharacterCommand(long accountId,
            long characterId)
        {
            AccountId = accountId;
            CharacterId = characterId;
        }
    }
}