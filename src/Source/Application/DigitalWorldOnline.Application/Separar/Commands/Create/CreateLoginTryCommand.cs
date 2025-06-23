using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.DTOs.Account;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Commands.Create
{
    public class CreateLoginTryCommand : IRequest<LoginTryDTO>
    {
        public string Username { get; set; }

        public string IpAddress { get; set; }

        public LoginTryResultEnum Result { get; set; }

        public CreateLoginTryCommand(string username, string ipAddress, LoginTryResultEnum result)
        {
            Username = username;
            IpAddress = ipAddress;
            Result = result;
        }
    }
}
