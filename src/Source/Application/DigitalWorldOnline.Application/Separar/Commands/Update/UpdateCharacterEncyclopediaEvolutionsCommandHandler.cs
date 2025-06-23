using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterEncyclopediaEvolutionsCommandHandler : IRequestHandler<UpdateCharacterEncyclopediaEvolutionsCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateCharacterEncyclopediaEvolutionsCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateCharacterEncyclopediaEvolutionsCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateCharacterEncyclopediaEvolutionsAsync(request.CharacterEncyclopediaEvolutions);

            return Unit.Value;
        }
    }
}