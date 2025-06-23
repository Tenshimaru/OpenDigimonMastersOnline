using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{

    public class ChangeTamerModelByIdCommandHandler : IRequestHandler<ChangeTamerModelByIdCommand, CharacterDTO>
    {
        private readonly ICharacterCommandsRepository _repository;

        public ChangeTamerModelByIdCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }


        public async Task<CharacterDTO> Handle(ChangeTamerModelByIdCommand request, CancellationToken cancellationToken)
        {
            return await _repository.ChangeTamerModelAsync(request.CharacterId, request.Model);
        }
    }
}