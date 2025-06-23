using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateTamerSkillCooldownByIdCommandHandler : IRequestHandler<UpdateTamerSkillCooldownByIdCommand>
    {
        private readonly ICharacterCommandsRepository _repository;

        public UpdateTamerSkillCooldownByIdCommandHandler(ICharacterCommandsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateTamerSkillCooldownByIdCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateTamerSkillCooldownAsync(request.ActiveSkill);

            return Unit.Value;
        }
    }

    
}