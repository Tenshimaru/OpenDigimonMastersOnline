using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetRaidAssetQueryHandler : IRequestHandler<GetRaidAssetQuery, GetRaidAssetQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetRaidAssetQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetRaidAssetQueryDto> Handle(GetRaidAssetQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetRaidBossAssetAsync(request.Filter);
        }
    }
}