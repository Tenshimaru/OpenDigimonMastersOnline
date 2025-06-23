using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventMapRaidsQueryHandler : IRequestHandler<GetEventMapRaidsQuery, GetEventMapRaidsQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetEventMapRaidsQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetEventMapRaidsQueryDto> Handle(GetEventMapRaidsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEventMapRaidsAsync(request.MapId, request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}