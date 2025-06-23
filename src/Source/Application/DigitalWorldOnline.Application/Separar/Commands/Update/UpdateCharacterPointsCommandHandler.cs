using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterArenaPointsCommandHandler : IRequestHandler<UpdateCharacterArenaPointsCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterArenaPointsCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterArenaPointsCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterArenaPointsAsync(request.Points);

            return Unit.Value;
        }
    }
}