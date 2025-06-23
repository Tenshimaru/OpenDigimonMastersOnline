using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateLastPlayedServerCommand : IRequest
    {
        public long AccountId { get; set; }

        public long ServerId { get; set; }

        public UpdateLastPlayedServerCommand(long accountId,
            long serverId)
        {
            AccountId = accountId;
            ServerId = serverId;
        }
    }
}