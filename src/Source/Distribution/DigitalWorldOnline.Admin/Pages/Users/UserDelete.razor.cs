using DigitalWorldOnline.Application.Admin.Commands;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Users
{
    public partial class UserDelete
    {
        bool _loading;
        long _id;

        [Parameter]
        public string UserId { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public ISender Sender { get; set; }

        [Inject]
        public ISnackbar Toast { get; set; }

        [Inject]
        public ILogger Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            long.TryParse(UserId, out _id);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid user id parameter: {parameter}", UserId);
                Toast.Add("User not found, try again later.", Severity.Warning);

                Return();
            }

            if (firstRender)
            {
                await Delete();
            }
        }

        private async Task Delete()
        {
            try
            {
                _loading = true;
                StateHasChanged();

                Logger.Information("Deleting user {userid}", _id);

                await Sender.Send(new DeleteUserCommand(_id));

                Logger.Information("User {userid} deleted.", _id);

                Toast.Add("User deleted.", Severity.Success);

                Nav.NavigateTo("/users");
            }
            catch (Exception ex)
            {
                Logger.Error("Error deleting user id {userId}: {ex}", _id, ex.Message);
                Toast.Add("Unable to delete user, try again later.", Severity.Error);
            }
            finally
            {
                _loading = false;
                StateHasChanged();

                Return();
            }
        }

        private void Return()
        {
            Nav.NavigateTo("/users");
        }
    }
}
