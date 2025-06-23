using DigitalWorldOnline.Commons.Enums;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class GetAdminAccessLevelQuery : IRequest<UserAccessLevelEnum>
    {
        public string Username { get; private set; }

        public GetAdminAccessLevelQuery(string username)
        {
            Username = username;
        }
    }
}