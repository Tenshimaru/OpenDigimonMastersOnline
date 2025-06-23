using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetItemAssetByIdQueryHandler : IRequestHandler<GetItemAssetByIdQuery, GetItemAssetByIdQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetItemAssetByIdQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetItemAssetByIdQueryDto> Handle(GetItemAssetByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetItemAssetByIdAsync(request.Id);
        }
    }
}