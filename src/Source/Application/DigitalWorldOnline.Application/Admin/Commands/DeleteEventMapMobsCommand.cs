using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteEventMapMobsCommand : IRequest
    {
        public long Id { get; set; }

        public DeleteEventMapMobsCommand(long id)
        {
            Id = id;
        }
    }
}