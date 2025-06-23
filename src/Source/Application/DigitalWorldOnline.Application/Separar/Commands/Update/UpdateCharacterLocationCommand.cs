using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterLocationCommand : IRequest
    {
        public CharacterLocationModel Location { get; set; }

        public UpdateCharacterLocationCommand(CharacterLocationModel location)
        {
            Location = location;
        }
    }
}