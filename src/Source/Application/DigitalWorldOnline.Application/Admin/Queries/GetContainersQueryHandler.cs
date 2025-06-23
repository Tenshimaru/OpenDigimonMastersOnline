using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetContainersQueryHandler : IRequestHandler<GetContainersQuery, GetContainersQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetContainersQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetContainersQueryDto> Handle(GetContainersQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetContainersAsync(request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}