using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Events;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterEncyclopediaEvolutionsCommand : IRequest
    {
        public CharacterEncyclopediaEvolutionsModel CharacterEncyclopediaEvolutions { get; set; }

        public UpdateCharacterEncyclopediaEvolutionsCommand(CharacterEncyclopediaEvolutionsModel characterEncyclopediaEvolutions)
        {
            CharacterEncyclopediaEvolutions = characterEncyclopediaEvolutions;
        }
    }
}
