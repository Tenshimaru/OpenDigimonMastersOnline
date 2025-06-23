using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetScansQueryHandler : IRequestHandler<GetScansQuery, GetScansQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetScansQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetScansQueryDto> Handle(GetScansQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetScansAsync(request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}