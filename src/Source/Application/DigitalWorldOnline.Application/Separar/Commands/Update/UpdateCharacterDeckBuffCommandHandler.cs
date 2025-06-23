using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterDeckBuffCommandHandler : IRequestHandler<UpdateCharacterDeckBuffCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterDeckBuffCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterDeckBuffCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterDeckBuffAsync(request.Character);

            return Unit.Value;
        }
    }
}