using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.ViewModel.User;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Users
{
    public partial class UserCreation
    {
        bool Loading = false;

        UserCreationViewModel _user = new UserCreationViewModel();

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public ISender Sender { get; set; }

        [Inject]
        public ISnackbar Toast { get; set; }

        [Inject]
        public ILogger Logger { get; set; }

        private async Task Create()
        {
            if (_user.Empty)
                return;

            try
            {
                Loading = true;

                StateHasChanged();

                Logger.Information("Creating new user with username {username}", _user.Username);

                await Sender.Send(
                    new CreateUserCommand(
                       _user.Username,
                       _user.Password.HashPassword(),
                       _user.AccessLevel)
                );

                Logger.Information("User created with username {username}", _user.Username);

                Toast.Add("User created successfully.", Severity.Success);

                Nav.NavigateTo("/users");
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating user with username {username}: {ex}", _user.Username, ex.Message);
                Toast.Add("Unable to create user, try again later.", Severity.Error);
            }
            finally
            {
                Loading = false;

                StateHasChanged();
            }
        }

        private void Return()
        {
            Nav.NavigateTo("/users");
        }
    }
}
