using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventRaidsQueryHandler : IRequestHandler<GetEventRaidsQuery, GetEventRaidsQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetEventRaidsQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetEventRaidsQueryDto> Handle(GetEventRaidsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEventRaidsAsync(request.MapId, request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}