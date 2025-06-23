using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetClonByIdQuery : IRequest<GetClonByIdQueryDto>
    {
        public long Id { get; }

        public GetClonByIdQuery(long id)
        {
            Id = id;
        }
    }
}