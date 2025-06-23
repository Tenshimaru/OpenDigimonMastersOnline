using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Delete
{
    public class DeleteBanCommand : IRequest
    {
        public long Id { get; set; }

        public DeleteBanCommand(long id)
        {
            Id = id;
        }
    }
}
