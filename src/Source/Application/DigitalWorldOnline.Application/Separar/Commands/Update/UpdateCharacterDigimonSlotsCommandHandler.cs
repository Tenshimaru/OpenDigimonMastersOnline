using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterDigimonSlotsCommandHandler : IRequestHandler<UpdateCharacterDigimonSlotsCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterDigimonSlotsCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterDigimonSlotsCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterDigimonSlotsAsync(request.CharacterId, request.Slots);

            return Unit.Value;
        }
    }
}