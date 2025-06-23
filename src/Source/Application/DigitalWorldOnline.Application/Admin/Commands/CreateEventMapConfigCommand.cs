using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateEventMapConfigCommand : IRequest<EventMapsConfigDTO>
    {
        public EventMapsConfigDTO EventMap { get; }

        public CreateEventMapConfigCommand(EventMapsConfigDTO eventMapConfig)
        {
            EventMap = eventMapConfig;
        }
    }
}