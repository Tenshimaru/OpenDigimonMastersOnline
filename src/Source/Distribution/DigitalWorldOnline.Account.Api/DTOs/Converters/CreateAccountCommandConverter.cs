using DigitalWorldOnline.Api.Dtos.In;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Commons.Extensions;

namespace DigitalWorldOnline.Api.Dtos.Converters
{
    public static class CreateAccountCommandConverter
    {
        public static CreateUserAccountCommand Convert(CreateAccountIn account)
        {
            return new CreateUserAccountCommand(
                account.Username.Base64Decrypt(),
                account.Email.Base64Decrypt(),
                account.DiscordId.Base64Decrypt(),
                account.Password.Base64Decrypt());
        }
    }
}
