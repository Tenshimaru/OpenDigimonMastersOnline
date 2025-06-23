using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateEventConfigCommand : IRequest
    {
        public EventConfigDTO Event { get; }

        public UpdateEventConfigCommand(EventConfigDTO eventConfig)
        {
            Event = eventConfig;
        }
    }
}