using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetSpawnPointByIdQuery : IRequest<GetSpawnPointByIdQueryDto>
    {
        public long Id { get; }

        public GetSpawnPointByIdQuery(long id)
        {
            Id = id;
        }
    }
}