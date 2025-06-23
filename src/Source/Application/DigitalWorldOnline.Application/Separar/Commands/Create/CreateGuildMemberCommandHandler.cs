using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateGuildMemberCommandHandler : IRequestHandler<CreateGuildMemberCommand>
    {
        private readonly IServerCommandsRepository _repository;

        public CreateGuildMemberCommandHandler(IServerCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreateGuildMemberCommand request, CancellationToken cancellationToken)
        {
            if (request.Member == null)
                return Unit.Value;

            await _repository.AddGuildMemberAsync(request.GuildId, request.Member);

            return Unit.Value;
        }
    }
}