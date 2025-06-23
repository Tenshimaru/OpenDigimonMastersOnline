using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries;

public class CreateGameAccountQueryHandler : IRequestHandler<CreateGameAccountQuery, AccountDTO>
{
    private readonly IAccountQueriesRepository _accountQueriesRepository;

    public CreateGameAccountQueryHandler(IAccountQueriesRepository accountQueriesRepository)
    {
        _accountQueriesRepository = accountQueriesRepository;
    }

    public async Task<AccountDTO> Handle(CreateGameAccountQuery request, CancellationToken cancellationToken)
    {
        return await _accountQueriesRepository.CreateGameAccountAsync(request.Username, request.Password);
    }
}