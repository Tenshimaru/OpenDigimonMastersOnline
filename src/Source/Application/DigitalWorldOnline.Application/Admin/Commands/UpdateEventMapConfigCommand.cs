using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateEventMapConfigCommand : IRequest
    {
        public EventMapsConfigDTO EventMap { get; }

        public UpdateEventMapConfigCommand(EventMapsConfigDTO eventMapConfig)
        {
            EventMap = eventMapConfig;
        }
    }
}