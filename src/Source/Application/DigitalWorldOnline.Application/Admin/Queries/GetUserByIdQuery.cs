using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetUserByIdQuery : IRequest<GetUserByIdQueryDto>
    {
        public long Id { get; }

        public GetUserByIdQuery(long id)
        {
            Id = id;
        }
    }
}