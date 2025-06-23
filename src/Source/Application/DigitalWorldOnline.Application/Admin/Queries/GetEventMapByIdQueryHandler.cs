using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventMapByIdQueryHandler : IRequestHandler<GetEventMapByIdQuery, GetEventMapByIdQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetEventMapByIdQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetEventMapByIdQueryDto> Handle(GetEventMapByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEventMapByIdAsync(request.Id);
        }
    }
}