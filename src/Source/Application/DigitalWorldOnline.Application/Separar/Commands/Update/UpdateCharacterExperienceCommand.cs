using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterExperienceCommand : IRequest
    {
        public long TamerId { get; set; }
        public long CurrentExperience { get; set; }
        public byte Level { get; set; }

        public UpdateCharacterExperienceCommand(long tamerId,
            long currentExperience, byte level)
        {
            TamerId = tamerId;
            CurrentExperience = currentExperience;
            Level = level;
        }

        public UpdateCharacterExperienceCommand(CharacterModel character)
        {
            TamerId = character.Id;
            CurrentExperience = character.CurrentExperience;
            Level = character.Level;
        }
    }
}
