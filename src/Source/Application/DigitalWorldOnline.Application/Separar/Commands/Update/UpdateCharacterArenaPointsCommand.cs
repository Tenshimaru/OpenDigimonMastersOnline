using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterArenaPointsCommand : IRequest
    {
        public CharacterArenaPointsModel Points { get; set; }

        public UpdateCharacterArenaPointsCommand(CharacterArenaPointsModel points)
        {
            Points = points;
        }
    }
}