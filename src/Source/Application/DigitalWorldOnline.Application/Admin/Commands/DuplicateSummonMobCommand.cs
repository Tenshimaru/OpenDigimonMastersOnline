using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DuplicateSummonMobCommand : IRequest
    {
        public long Id { get; set; }

        public DuplicateSummonMobCommand(long id)
        {
            Id = id;
        }
    }
}