using MediatR;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class EventsConfigQuery : IRequest<List<EventConfigDTO>>
    {
        public bool IsEnabled { get; private set; }

        public EventsConfigQuery(bool isEnabled = true)
        {
            IsEnabled = isEnabled;
        }
    }
}