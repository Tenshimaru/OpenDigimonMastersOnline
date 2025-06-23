using DigitalWorldOnline.Commons.Enums.Account;
using FluentValidation;
using MediatR;

namespace DigitalWorldOnline.Application.Admin.Commands
{
    public class CreateUserAccountCommand : IRequest<AccountCreateResult>
    {
        public string Username { get; }
        public string Email { get; }
        public string DiscordId { get; }
        public string Password { get; }

        public CreateUserAccountCommand(string username, string email, string discordId, string password)
        {
            Username = username;
            Email = email;
            DiscordId = discordId;
            Password = password;
        }
    }

    public class CreateUserAccountCommandValidator : AbstractValidator<CreateUserAccountCommand>
    {
        public CreateUserAccountCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(15)
                .WithMessage("Invalid username (6-15 characters).");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(50)
                .WithMessage("Invalid e-mail address.");

            RuleFor(x => x.DiscordId)
                .NotEmpty()
                .WithMessage("Required DiscordId.");
            
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(12)
                .WithMessage("Invalid password (6-12 characters).");
        }
    }
}