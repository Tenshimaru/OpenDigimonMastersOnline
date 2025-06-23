using DigitalWorldOnline.Commons.Models.Character;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateCharacterMapRegionCommand : IRequest
    {
        public CharacterMapRegionModel MapRegion { get; }

        public UpdateCharacterMapRegionCommand(CharacterMapRegionModel mapRegion)
        {
            MapRegion = mapRegion;
        }
    }
}