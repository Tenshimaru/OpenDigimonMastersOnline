using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class StaffAccountsQueryHandler : IRequestHandler<StaffAccountsQuery, IList<AccountDTO>>
    {
        private readonly IServerQueriesRepository _repository;

        public StaffAccountsQueryHandler(IServerQueriesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<AccountDTO>> Handle(StaffAccountsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetStaffAccountsAsync();
        }
    }
}
