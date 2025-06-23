using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterActiveEvolutionCommandHandler : IRequestHandler<UpdateCharacterActiveEvolutionCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterActiveEvolutionCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterActiveEvolutionCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterActiveEvolutionAsync(request.ActiveEvolution);

            return Unit.Value;
        }
    }
}