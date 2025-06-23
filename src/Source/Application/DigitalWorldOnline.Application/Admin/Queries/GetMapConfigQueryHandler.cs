using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetMapConfigQueryHandler : IRequestHandler<GetMapConfigQuery, GetMapConfigQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetMapConfigQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetMapConfigQueryDto> Handle(GetMapConfigQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetMapConfigAsync(request.Filter);
        }
    }
}