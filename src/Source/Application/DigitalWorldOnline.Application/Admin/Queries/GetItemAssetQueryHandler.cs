using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetItemAssetQueryHandler : IRequestHandler<GetItemAssetQuery, GetItemAssetQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetItemAssetQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetItemAssetQueryDto> Handle(GetItemAssetQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetItemAssetAsync(request.Filter);
        }
    }
}