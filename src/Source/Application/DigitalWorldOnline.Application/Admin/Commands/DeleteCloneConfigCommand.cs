using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteCloneConfigCommand : IRequest
    {
        public long Id { get; set; }

        public DeleteCloneConfigCommand(long id)
        {
            Id = id;
        }
    }
}