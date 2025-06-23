using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetMobByIdQuery : IRequest<GetMobByIdQueryDto>
    {
        public long Id { get; }

        public GetMobByIdQuery(long id)
        {
            Id = id;
        }
    }
}