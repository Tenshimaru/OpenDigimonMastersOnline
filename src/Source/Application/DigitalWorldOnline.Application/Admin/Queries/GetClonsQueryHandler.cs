using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetClonsQueryHandler : IRequestHandler<GetClonsQuery, GetClonsQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetClonsQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetClonsQueryDto> Handle(GetClonsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetClonsAsync(request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}