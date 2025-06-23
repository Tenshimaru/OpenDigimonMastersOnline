using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetPlayerByIdQueryHandler : IRequestHandler<GetPlayerByIdQuery, GetPlayerByIdQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetPlayerByIdQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetPlayerByIdQueryDto> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetPlayerByIdAsync(request.PlayerId);
        }
    }
}
