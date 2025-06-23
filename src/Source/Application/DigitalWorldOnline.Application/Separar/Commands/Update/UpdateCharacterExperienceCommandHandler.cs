using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterExperienceCommandHandler : IRequestHandler<UpdateCharacterExperienceCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterExperienceCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterExperienceCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterExperienceAsync(request.TamerId, request.CurrentExperience, request.Level);

            return Unit.Value;
        }
    }
}
