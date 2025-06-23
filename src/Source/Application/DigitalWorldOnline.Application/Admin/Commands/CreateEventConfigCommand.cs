using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateEventConfigCommand : IRequest<EventConfigDTO>
    {
        public EventConfigDTO Event { get; }

        public CreateEventConfigCommand(EventConfigDTO eventConfig)
        {
            Event = eventConfig;
        }
    }
}