using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateAccountWelcomeFlagCommandHandler : IRequestHandler<UpdateAccountWelcomeFlagCommand>
    {
        private readonly IAccountCommandsRepository _repository;

        public UpdateAccountWelcomeFlagCommandHandler(IAccountCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateAccountWelcomeFlagCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateAccountWelcomeFlagAsync(request.AccountId, request.WelcomeFlag);

            return Unit.Value;
        }
    }
}