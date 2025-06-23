using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DuplicateEventMobCommand : IRequest
    {
        public long Id { get; set; }

        public DuplicateEventMobCommand(long id)
        {
            Id = id;
        }
    }
}