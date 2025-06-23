using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Delete
{
    public class DeleteGuildMemberCommandHandler : IRequestHandler<DeleteGuildMemberCommand>
    {
        private readonly IServerCommandsRepository _repository;

        public DeleteGuildMemberCommandHandler(IServerCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteGuildMemberCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteGuildMemberAsync(request.CharacterId, request.GuildId);

            return Unit.Value;
        }
    }
}
