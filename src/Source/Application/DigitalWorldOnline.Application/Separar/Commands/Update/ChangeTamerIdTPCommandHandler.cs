using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class ChangeTamerIdTPCommandHandler : IRequestHandler<ChangeTamerIdTPCommand, CharacterDTO>
    {
        private readonly ICharacterCommandsRepository _repository;

        public ChangeTamerIdTPCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<CharacterDTO> Handle(ChangeTamerIdTPCommand request, CancellationToken cancellationToken)
        {
            return await _repository.ChangeCharacterIdTpAsync(request.CharacterId, request.NewTargetTamerIdTP);
        }
    }
}