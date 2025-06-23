using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateDigimonExperienceCommandHandler : IRequestHandler<UpdateDigimonExperienceCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateDigimonExperienceCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDigimonExperienceCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateDigimonExperienceAsync(request.Digimon);

            return Unit.Value;
        }
    }
}
