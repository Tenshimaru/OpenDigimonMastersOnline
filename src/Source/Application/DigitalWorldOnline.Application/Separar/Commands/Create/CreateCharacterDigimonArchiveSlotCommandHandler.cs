using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateCharacterDigimonArchiveSlotCommandHandler : IRequestHandler<CreateCharacterDigimonArchiveSlotCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public CreateCharacterDigimonArchiveSlotCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreateCharacterDigimonArchiveSlotCommand request, CancellationToken cancellationToken)
        {
            await _repository.AddDigimonArchiveSlotAsync(request.ArchiveId, request.ArchiveItem);

            return Unit.Value;
        }
    }
}