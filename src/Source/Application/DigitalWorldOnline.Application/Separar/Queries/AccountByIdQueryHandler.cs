using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AccountByIdQueryHandler : IRequestHandler<AccountByIdQuery, AccountDTO?>
    {
        private readonly IAccountQueriesRepository _repository;

        public AccountByIdQueryHandler(IAccountQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountDTO?> Handle(AccountByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAccountByIdAsync(request.Id);
        }
    }
}
