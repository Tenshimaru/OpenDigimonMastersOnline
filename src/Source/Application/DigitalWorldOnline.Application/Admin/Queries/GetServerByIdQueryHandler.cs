using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetServerByIdQueryHandler : IRequestHandler<GetServerByIdQuery, GetServerByIdQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetServerByIdQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetServerByIdQueryDto> Handle(GetServerByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetServerByIdAsync(request.Id);
        }
    }
}