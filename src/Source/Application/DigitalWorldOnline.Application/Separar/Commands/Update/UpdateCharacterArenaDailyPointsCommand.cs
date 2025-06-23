using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Mechanics;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterArenaDailyPointsCommand : IRequest
    {
        public CharacterArenaDailyPointsModel Points { get; set; }

        public UpdateCharacterArenaDailyPointsCommand(CharacterArenaDailyPointsModel points)
        {
            Points = points;
        }
    }
}