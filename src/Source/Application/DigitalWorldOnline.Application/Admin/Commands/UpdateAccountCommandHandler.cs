using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand>
    {
        private readonly IAdminCommandsRepository _repository;

        public UpdateAccountCommandHandler(IAdminCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var dto = new AccountDTO()
            {
                Id = request.Id,
                Username = request.Username,
                Email = request.Email,
                SecondaryPassword = request.SecondaryPassword,
                DiscordId = request.DiscordId ?? string.Empty,
                Premium = request.Premium,
                Silk = request.Silk,
                AccessLevel = request.AccessLevel,
                MembershipExpirationDate = request.MembershipExpirationDate,
                ReceiveWelcome = request.ReceiveWelcome
            };

            await _repository.UpdateAccountAsync(dto);

            return Unit.Value;
        }
    }
}