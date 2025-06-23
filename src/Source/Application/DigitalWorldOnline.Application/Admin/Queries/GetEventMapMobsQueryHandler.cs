using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventMapMobsQueryHandler : IRequestHandler<GetEventMapMobsQuery, GetEventMapMobsQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetEventMapMobsQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetEventMapMobsQueryDto> Handle(GetEventMapMobsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEventMapMobsAsync(request.MapId, request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}