using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteSpawnPointCommand : IRequest
    {
        public long Id { get; set; }

        public DeleteSpawnPointCommand(long id)
        {
            Id = id;
        }
    }
}