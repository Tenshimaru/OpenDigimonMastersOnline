using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterProgressCompleteCommandHandler : IRequestHandler<UpdateCharacterProgressCompleteCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterProgressCompleteCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterProgressCompleteCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterProgressCompleteAsync(request.Progress);

            return Unit.Value;
        }
    }
}