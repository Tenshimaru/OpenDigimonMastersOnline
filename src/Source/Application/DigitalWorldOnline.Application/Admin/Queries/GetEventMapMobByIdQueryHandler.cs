using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventMapMobByIdQueryHandler : IRequestHandler<GetEventMapMobByIdQuery, GetEventMapMobByIdQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetEventMapMobByIdQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetEventMapMobByIdQueryDto> Handle(GetEventMapMobByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEventMapMobByIdAsync(request.Id);
        }
    }
}