using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetSpawnPointsQueryHandler : IRequestHandler<GetSpawnPointsQuery, GetSpawnPointsQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetSpawnPointsQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetSpawnPointsQueryDto> Handle(GetSpawnPointsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetSpawnPointsAssetAsync(request.MapId, request.Limit, request.Offset, request.SortColumn, request.SortDirection);
        }
    }
}