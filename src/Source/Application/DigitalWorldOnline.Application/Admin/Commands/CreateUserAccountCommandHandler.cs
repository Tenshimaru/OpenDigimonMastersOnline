using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateUserAccountCommandHandler : IRequestHandler<CreateUserAccountCommand, AccountCreateResult>
    {
        private readonly IAdminCommandsRepository _repository;
        //private readonly IEmailService _emailService;

        public CreateUserAccountCommandHandler(
            IAdminCommandsRepository repository)
        //IEmailService emailService)
        {
            _repository = repository;
            //_emailService = emailService;
        }

        public async Task<AccountCreateResult> Handle(CreateUserAccountCommand request, CancellationToken cancellationToken)
        {
            var result = await _repository.CreateAccountAsync(request.Username, request.Email, request.DiscordId, request.Password);

            if (result == AccountCreateResult.Created)
            {
                //_emailService.Send(request.Email);
            }

            return result;
        }
    }
}