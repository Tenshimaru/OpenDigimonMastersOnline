using MediatR;
using DigitalWorldOnline.Application.Admin.Repositories;

namespace DigitalWorldOnline.Application.Admin.Queries
{
    public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, GetAccountByIdQueryDto>
    {
        private readonly IAdminQueriesRepository _repository;

        public GetAccountByIdQueryHandler(IAdminQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAccountByIdQueryDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAccountByIdAsync(request.Id);
        }
    }
}