using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterEncyclopediaCommandHandler : IRequestHandler<UpdateCharacterEncyclopediaCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterEncyclopediaCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterEncyclopediaCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterEncyclopediaAsync(request.CharacterEncyclopedia);

            return Unit.Value;
        }
    }
}