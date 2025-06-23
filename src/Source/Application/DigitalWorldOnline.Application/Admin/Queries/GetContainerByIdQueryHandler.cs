using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetContainerByIdQueryHandler : IRequestHandler<GetContainerByIdQuery, GetContainerByIdQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetContainerByIdQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetContainerByIdQueryDto> Handle(GetContainerByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetContainerByIdAsync(request.Id);
        }
    }
}