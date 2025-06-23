using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateArenaRankingCommandHandler : IRequestHandler<UpdateArenaRankingCommand>
    {
        private readonly IServerCommandsRepository _repository;

        public UpdateArenaRankingCommandHandler(IServerCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateArenaRankingCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateArenaRankingAsync(request.Arena);

            return Unit.Value;
        }
    }
}