using DigitalWorldOnline.Commons.Models.Config;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateMapConfigCommand : IRequest
    {
        public MapConfigModel MapConfig { get; set; }

        public UpdateMapConfigCommand(MapConfigModel mapConfig)
        {
            MapConfig = mapConfig;
        }
    }
}
