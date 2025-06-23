using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, GetEventsQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetEventsQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetEventsQueryDto> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEventsAsync(request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}