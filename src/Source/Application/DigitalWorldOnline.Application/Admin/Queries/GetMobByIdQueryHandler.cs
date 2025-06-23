using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetMobByIdQueryHandler : IRequestHandler<GetMobByIdQuery, GetMobByIdQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetMobByIdQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetMobByIdQueryDto> Handle(GetMobByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetMobByIdAsync(request.Id);
        }
    }
}