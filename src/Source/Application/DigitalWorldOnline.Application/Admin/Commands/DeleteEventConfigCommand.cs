using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteEventConfigCommand : IRequest
    {
        public long Id { get; set; }

        public DeleteEventConfigCommand(long id)
        {
            Id = id;
        }
    }
}