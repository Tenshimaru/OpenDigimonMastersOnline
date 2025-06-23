using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class DeleteSummonCommand : IRequest
    {
        public long Id { get; set; }

        public DeleteSummonCommand(long id)
        {
            Id = id;
        }
    }
}