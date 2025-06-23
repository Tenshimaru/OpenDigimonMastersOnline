using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.ViewModel.User;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Users
{
    public partial class UserUpdate
    {
        UserUpdateViewModel _user = new ();
        bool _loading = true;
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

            if (long.TryParse(UserId, out _id))
            {
                Logger.Information("Searching user by id {userid}", _id);

                var targetUser = await Sender.Send(new GetUserByIdQuery(_id));

                _user = targetUser.Register != null ? 
                    new UserUpdateViewModel(
                        targetUser.Register.Id,
                        targetUser.Register.Username,
                        targetUser.Register.AccessLevel) : null;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0 || _user == null)
            {
                Logger.Information("Invalid user id parameter: {parameter}", UserId);
                Toast.Add("User not found, try again later.", Severity.Warning);

                Return();
            }

            if(firstRender)
            {
                _loading = false;
                StateHasChanged();
            }
        }

        private async Task Update()
        {
            if (_user.Empty)
                return;

            try
            {
                _loading = true;

                StateHasChanged();

                Logger.Information("Updating user {userid}", _user.Id);

                await Sender.Send(
                    new UpdateUserCommand(
                        _user.Id,
                        _user.Username,
                        _user.AccessLevel)
                );

                Logger.Information("User {userid} updated.", _user.Id);

                Toast.Add("User updated.", Severity.Success);

                Nav.NavigateTo("/users");
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating user id {userId}: {ex}", _user.Id, ex.Message);
                Toast.Add("Unable to update user, try again later.", Severity.Error);
            }
            finally
            {
                _loading = false;

                StateHasChanged();
            }
        }

        private void Return()
        {
            Nav.NavigateTo("/users");
        }
    }
}
