using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CharactersInServerQuery : IRequest<byte>
    {
        public long AccountId { get; set; }

        public long ServerId { get; set; }

        public CharactersInServerQuery(long accountId, long serverId)
        {
            AccountId = accountId;
            ServerId = serverId;
        }
    }
}