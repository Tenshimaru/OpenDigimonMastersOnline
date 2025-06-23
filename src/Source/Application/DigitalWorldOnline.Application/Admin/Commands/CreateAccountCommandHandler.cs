using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDTO>
    {
        private readonly IAdminCommandsRepository _repository;

        public CreateAccountCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountDTO> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddAccountAsync(request.Account);
        }
    }
}