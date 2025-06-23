using DigitalWorldOnline.Commons.Models.Config;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Update
{
    public class UpdateAdminUserCommand : IRequest
    {
        public AdminUserModel User { get; private set; }

        public UpdateAdminUserCommand(AdminUserModel user)
        {
            User = user;
        }
    }
}