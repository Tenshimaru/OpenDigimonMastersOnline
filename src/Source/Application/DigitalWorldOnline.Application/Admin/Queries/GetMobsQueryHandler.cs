using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetMobsQueryHandler : IRequestHandler<GetMobsQuery, GetMobsQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetMobsQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetMobsQueryDto> Handle(GetMobsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetMobsAsync(request.MapId, request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}