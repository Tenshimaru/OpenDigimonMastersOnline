using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetEventConfigByIdQueryHandler : IRequestHandler<GetEventConfigByIdQuery, GetEventConfigByIdQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetEventConfigByIdQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetEventConfigByIdQueryDto> Handle(GetEventConfigByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEventConfigByIdAsync(request.Id);
        }
    }
}