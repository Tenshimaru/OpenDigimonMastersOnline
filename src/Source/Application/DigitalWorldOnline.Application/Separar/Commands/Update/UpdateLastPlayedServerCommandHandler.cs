using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateLastPlayedServerCommandHandler : IRequestHandler<UpdateLastPlayedServerCommand>
    {
        private readonly IAccountCommandsRepository _repository;

        public UpdateLastPlayedServerCommandHandler(IAccountCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateLastPlayedServerCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateLastPlayedServerByIdAsync(request.AccountId, request.ServerId);

            return Unit.Value;
        }
    }
}