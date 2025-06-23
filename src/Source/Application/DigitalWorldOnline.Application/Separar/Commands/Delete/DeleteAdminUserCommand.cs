using DigitalWorldOnline.Commons.Models.Config;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Delete
{
    public class DeleteAdminUserCommand : IRequest
    {
        public long UserId { get; private set; }

        public DeleteAdminUserCommand(long userId)
        {
            UserId = userId;
        }
    }
}