using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetSummonsQueryHandler : IRequestHandler<GetSummonsQuery, GetSummonsQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetSummonsQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetSummonsQueryDto> Handle(GetSummonsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetSummonsAsync(request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}