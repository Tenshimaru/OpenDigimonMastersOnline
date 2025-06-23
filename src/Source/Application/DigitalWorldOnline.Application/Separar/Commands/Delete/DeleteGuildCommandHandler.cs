using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Delete
{
    public class DeleteGuildCommandHandler : IRequestHandler<DeleteGuildCommand>
    {
        private readonly IServerCommandsRepository _repository;

        public DeleteGuildCommandHandler(IServerCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteGuildCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteGuildAsync(request.GuildId);

            return Unit.Value;
        }
    }
}
