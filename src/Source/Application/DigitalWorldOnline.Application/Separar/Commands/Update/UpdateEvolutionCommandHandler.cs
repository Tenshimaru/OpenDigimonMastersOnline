using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateEvolutionCommandHandler : IRequestHandler<UpdateEvolutionCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateEvolutionCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateEvolutionCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateEvolutionAsync(request.Evolution);

            return Unit.Value;
        }
    }
}