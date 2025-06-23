using MediatR;
using DigitalWorldOnline.Commons.DTOs.Account;

namespace DigitalWorldOnline.Application.Separar.Queries
{
    public class AccountByUsernameQuery : IRequest<AccountDTO?>
    {
        public string Username { get; set; }

        public AccountByUsernameQuery(string username)
        {
            Username = username;
        }
    }
}

