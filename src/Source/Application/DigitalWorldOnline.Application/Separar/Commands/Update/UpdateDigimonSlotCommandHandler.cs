using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateDigimonSlotCommandHandler : IRequestHandler<UpdateDigimonSlotCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateDigimonSlotCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDigimonSlotCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateDigimonSlotAsync(request.DigimonId, request.DigimonSlot);

            return Unit.Value;
        }
    }
}
