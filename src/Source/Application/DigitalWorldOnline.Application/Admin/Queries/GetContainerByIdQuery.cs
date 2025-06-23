using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetContainerByIdQuery : IRequest<GetContainerByIdQueryDto>
    {
        public long Id { get; }

        public GetContainerByIdQuery(long id)
        {
            Id = id;
        }
    }
}