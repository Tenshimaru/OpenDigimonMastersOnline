using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateEventMobCommand : IRequest<EventMobConfigDTO>
    {
        public EventMobConfigDTO Mob { get; }

        public CreateEventMobCommand(EventMobConfigDTO mob)
        {
            Mob = mob;
        }
    }
}