using DigitalWorldOnline.Commons.Enums;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class CheckPortalAccessQuery : IRequest<UserAccessLevelEnum>
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public CheckPortalAccessQuery(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}