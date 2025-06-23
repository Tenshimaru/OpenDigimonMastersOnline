using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DuplicateMobCommand : IRequest
    {
        public long Id { get; set; }

        public DuplicateMobCommand(long id)
        {
            Id = id;
        }
    }
}