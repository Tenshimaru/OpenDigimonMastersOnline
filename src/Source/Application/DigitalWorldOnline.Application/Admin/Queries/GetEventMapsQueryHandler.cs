using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventMapsQueryHandler : IRequestHandler<GetEventMapsQuery, GetEventMapsQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetEventMapsQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetEventMapsQueryDto> Handle(GetEventMapsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEventMapsAsync(request.EventId,request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}