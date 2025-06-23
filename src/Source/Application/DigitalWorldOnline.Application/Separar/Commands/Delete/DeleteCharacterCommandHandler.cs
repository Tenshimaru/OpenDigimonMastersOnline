using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Delete
{
    public class DeleteCharacterCommandHandler : IRequestHandler<DeleteCharacterCommand, DeleteCharacterResultEnum>
    {
        private readonly ICharacterCommandsRepository _repository;

        public DeleteCharacterCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeleteCharacterResultEnum> Handle(DeleteCharacterCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteCharacterByAccountAndPositionAsync(request.AccountId, request.CharacterPosition);
        }
    }
}
