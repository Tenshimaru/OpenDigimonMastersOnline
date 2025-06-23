using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetMapByIdQuery : IRequest<GetMapByIdQueryDto>
    {
        public long Id { get; }

        public GetMapByIdQuery(long id)
        {
            Id = id;
        }
    }
}