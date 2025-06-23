using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetSummonMobsQueryHandler : IRequestHandler<GetSummonMobsQuery, GetSummonMobsQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetSummonMobsQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetSummonMobsQueryDto> Handle(GetSummonMobsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetSummonMobsAsync(request.SummonDTOId, request.Limit, request.Offset, request.SortColumn, request.SortDirection, request.Filter);
        }
    }
}