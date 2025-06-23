using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AllAccountsQueryHandler : IRequestHandler<AllAccountsQuery, IList<AccountDTO>>
    {
        private readonly IAccountQueriesRepository _repository;

        public AllAccountsQueryHandler(IAccountQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<AccountDTO>> Handle(AllAccountsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAccountsAsync();
        }
    }
}
