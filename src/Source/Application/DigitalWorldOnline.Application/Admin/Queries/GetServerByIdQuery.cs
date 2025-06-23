using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetServerByIdQuery : IRequest<GetServerByIdQueryDto>
    {
        public long Id { get; }

        public GetServerByIdQuery(long id)
        {
            Id = id;
        }
    }
}