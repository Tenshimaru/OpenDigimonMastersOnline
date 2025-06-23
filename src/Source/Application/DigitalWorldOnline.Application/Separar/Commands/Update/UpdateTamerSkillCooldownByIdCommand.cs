using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Model.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateTamerSkillCooldownByIdCommand : IRequest
    {
        public CharacterTamerSkillModel ActiveSkill { get; set; }   

        public UpdateTamerSkillCooldownByIdCommand(CharacterTamerSkillModel activeSkill)
        {
            ActiveSkill = activeSkill;
        }
    }
}
