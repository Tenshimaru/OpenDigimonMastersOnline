using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetScanByIdQueryHandler : IRequestHandler<GetScanByIdQuery, GetScanByIdQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetScanByIdQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetScanByIdQueryDto> Handle(GetScanByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetScanByIdAsync(request.Id);
        }
    }
}