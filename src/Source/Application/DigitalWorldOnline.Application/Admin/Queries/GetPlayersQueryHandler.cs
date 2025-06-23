using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetPlayersQueryHandler : IRequestHandler<GetPlayersQuery, GetPlayersQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetPlayersQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetPlayersQueryDto> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetPlayersAsync(request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}