using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteServerCommand : IRequest
    {
        public long Id { get; set; }

        public DeleteServerCommand(long id)
        {
            Id = id;
        }
    }
}