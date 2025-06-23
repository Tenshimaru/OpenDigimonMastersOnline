using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class ChangeDigimonNameByIdCommandHandler : IRequestHandler<ChangeDigimonNameByIdCommand, DigimonDTO>
    {
        private readonly ICharacterCommandsRepository _repository;

        public ChangeDigimonNameByIdCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<DigimonDTO> Handle(ChangeDigimonNameByIdCommand request, CancellationToken cancellationToken)
        {
            return await _repository.ChangeDigimonNameAsync(request.DigimonId, request.NewDigimonName);
        }
    }
}