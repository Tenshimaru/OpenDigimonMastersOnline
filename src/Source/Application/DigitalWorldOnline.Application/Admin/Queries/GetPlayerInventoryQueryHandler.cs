using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetPlayerInventoryQueryHandler : IRequestHandler<GetPlayerInventoryQuery, GetPlayerInventoryQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetPlayerInventoryQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetPlayerInventoryQueryDto> Handle(GetPlayerInventoryQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetPlayerInventoryAsync(request.PlayerId);
        }
    }
}
