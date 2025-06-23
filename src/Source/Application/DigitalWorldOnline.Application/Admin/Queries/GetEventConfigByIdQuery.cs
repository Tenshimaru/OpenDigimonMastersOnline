using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventConfigByIdQuery : IRequest<GetEventConfigByIdQueryDto>
    {
        public long Id { get; }

        public GetEventConfigByIdQuery(long id)
        {
            Id = id;
        }
    }
}