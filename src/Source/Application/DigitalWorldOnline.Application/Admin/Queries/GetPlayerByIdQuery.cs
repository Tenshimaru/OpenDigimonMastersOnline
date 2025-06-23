using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetPlayerByIdQuery : IRequest<GetPlayerByIdQueryDto>
    {
        public long PlayerId { get; }

        public GetPlayerByIdQuery(long playerId)
        {
            PlayerId = playerId;
        }
    }
}
