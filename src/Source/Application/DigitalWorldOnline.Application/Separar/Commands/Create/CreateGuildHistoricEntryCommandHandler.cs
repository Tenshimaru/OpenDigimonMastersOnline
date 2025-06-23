using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateGuildHistoricEntryCommandHandler : IRequestHandler<CreateGuildHistoricEntryCommand>
    {
        private readonly IServerCommandsRepository _repository;

        public CreateGuildHistoricEntryCommandHandler(IServerCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreateGuildHistoricEntryCommand request, CancellationToken cancellationToken)
        {
            if (request.HistoricEntry == null)
                return Unit.Value;

            await _repository.AddGuildHistoricEntryAsync(request.GuildId, request.HistoricEntry);

            return Unit.Value;
        }
    }
}