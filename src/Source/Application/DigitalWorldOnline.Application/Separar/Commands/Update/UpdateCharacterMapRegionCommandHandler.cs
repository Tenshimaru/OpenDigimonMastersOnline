using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterMapRegionCommandHandler : IRequestHandler<UpdateCharacterMapRegionCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterMapRegionCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterMapRegionCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterMapRegionAsync(request.MapRegion);

            return Unit.Value;
        }
    }
}