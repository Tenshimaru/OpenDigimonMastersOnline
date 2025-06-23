using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterDigimonArchiveItemCommandHandler : IRequestHandler<UpdateCharacterDigimonArchiveItemCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterDigimonArchiveItemCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterDigimonArchiveItemCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterDigimonArchiveItemAsync(request.CharacterDigimonArchiveItem);

            return Unit.Value;
        }
    }
}