using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Repositories.Admin;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class ChangeTamerNameByIdCommandHandler : IRequestHandler<ChangeTamerNameByIdCommand, CharacterDTO>
    {
        private readonly ICharacterCommandsRepository _repository;

        public ChangeTamerNameByIdCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<CharacterDTO> Handle(ChangeTamerNameByIdCommand request, CancellationToken cancellationToken)
        {
            return await _repository.ChangeCharacterNameAsync(request.CharacterId, request.NewCharacterName);
        }
    }
}