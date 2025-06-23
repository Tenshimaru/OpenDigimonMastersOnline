using MediatR;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetMapConfigQuery : IRequest<GetMapConfigQueryDto>
    {
        public string Filter { get; }

        public GetMapConfigQuery(string filter)
        {
            Filter = filter;
        }
    }
}