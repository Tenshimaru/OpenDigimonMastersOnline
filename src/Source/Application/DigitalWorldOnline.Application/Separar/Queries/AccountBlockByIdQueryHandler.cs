using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AccountBlockByIdQueryHandler : IRequestHandler<AccountBlockByIdQuery, AccountBlockDTO?>
    {
        private readonly IAccountQueriesRepository _repository;

        public AccountBlockByIdQueryHandler(IAccountQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountBlockDTO?> Handle(AccountBlockByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAccountBlockByIdAsync(request.Id);
        }
    }
}
