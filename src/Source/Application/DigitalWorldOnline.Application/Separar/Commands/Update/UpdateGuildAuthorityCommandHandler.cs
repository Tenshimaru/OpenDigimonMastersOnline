using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateGuildAuthorityCommandHandler : IRequestHandler<UpdateGuildAuthorityCommand>
    {
        private readonly IServerCommandsRepository _repository;

        public UpdateGuildAuthorityCommandHandler(IServerCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateGuildAuthorityCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateGuildAuthorityAsync(request.Authority);

            return Unit.Value;
        }
    }
}