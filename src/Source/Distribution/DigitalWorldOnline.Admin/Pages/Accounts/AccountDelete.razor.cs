using DigitalWorldOnline.Application.Admin.Commands;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Accounts
{
    public partial class AccountDelete
    {
        bool _loading;
        long _id;

        [Parameter]
        public string AccountId { get; set; }

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

            long.TryParse(AccountId, out _id);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid account id parameter: {parameter}", AccountId);
                Toast.Add("Account not found, try again later.", Severity.Warning);

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

                Logger.Information("Deleting account {id}", _id);

                await Sender.Send(new DeleteAccountCommand(_id));

                Logger.Information("Account {id} deleted.", _id);

                Toast.Add("Account deleted.", Severity.Success);

                Nav.NavigateTo("/accounts");
            }
            catch (Exception ex)
            {
                Logger.Error("Error deleting account id {id}: {ex}", _id, ex.Message);
                Toast.Add("Unable to delete account, try again later.", Severity.Error);
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
            Nav.NavigateTo("/accounts");
        }
    }
}
