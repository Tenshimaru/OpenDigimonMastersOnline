using DigitalWorldOnline.Commons.Enums;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class UpdateUserCommand : IRequest
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public UserAccessLevelEnum AccessLevel { get; set; }

        public UpdateUserCommand(
            long id,
            string username,
            UserAccessLevelEnum accessLevel)
        {
            Id = id;
            UserName = username;
            AccessLevel = accessLevel;
        }
    }
}