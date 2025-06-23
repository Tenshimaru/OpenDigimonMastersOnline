using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteEventMapConfigCommand : IRequest
    {
        public long Id { get; set; }

        public DeleteEventMapConfigCommand(long id)
        {
            Id = id;
        }
    }
}