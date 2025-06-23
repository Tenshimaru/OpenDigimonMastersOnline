using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateDigimonCommandHandler : IRequestHandler<CreateDigimonCommand, DigimonDTO>
    {
        private readonly ICharacterCommandsRepository _repository;

        public CreateDigimonCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<DigimonDTO> Handle(CreateDigimonCommand request, CancellationToken cancellationToken)
        {
            return await _repository.AddDigimonAsync(request.Digimon);
        }
    }
}