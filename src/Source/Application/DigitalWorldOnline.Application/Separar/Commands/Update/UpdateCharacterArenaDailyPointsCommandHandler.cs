using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterArenaDailyPointsCommandHandler : IRequestHandler<UpdateCharacterArenaDailyPointsCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterArenaDailyPointsCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterArenaDailyPointsCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterArenaDailyPointsAsync(request.Points);

            return Unit.Value;
        }
    }
}