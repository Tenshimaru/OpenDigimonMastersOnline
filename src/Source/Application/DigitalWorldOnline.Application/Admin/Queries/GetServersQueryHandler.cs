using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetServersQueryHandler : IRequestHandler<GetServersQuery, GetServersQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetServersQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetServersQueryDto> Handle(GetServersQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetServersAsync(request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}