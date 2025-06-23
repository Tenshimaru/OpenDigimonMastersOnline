using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.ViewModel.Account;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.DTOs.Account;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Accounts
{
    public partial class AccountUpdate
    {
        AccountUpdateViewModel _account = new();
        AccountBlockDTO? _accountBlock = null;
        BanAccountModel _banModel = new();
        bool _loading = true;
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

            if (long.TryParse(AccountId, out _id))
            {
                Logger.Information("Searching account by id {id}", _id);

                var target = await Sender.Send(
                    new GetAccountByIdQuery(_id)
                );

                _account = target.Register != null ?
                    new AccountUpdateViewModel(
                        target.Register.Id,
                        target.Register.Username,
                        target.Register.Email,
                        target.Register.SecondaryPassword,
                        target.Register.DiscordId,
                        target.Register.AccessLevel,
                        target.Register.Premium,
                        target.Register.Silk,
                        target.Register.MembershipExpirationDate,
                        target.Register.ReceiveWelcome,
                        target.Register.CreateDate,
                        target.Register.LastConnection)
                    : null;

                // Load account block information if exists
                // TODO: Implement GetAccountBlockByAccountIdQuery
                // _accountBlock = await Sender.Send(new GetAccountBlockByAccountIdQuery(_id));

                // Initialize ban model
                _banModel.AccountId = _id;
                _banModel.StartDate = DateTime.Now;
                _banModel.EndDate = DateTime.Now.AddDays(7);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0 || _account == null)
            {
                Logger.Information("Invalid account id parameter: {parameter}", AccountId);
                Toast.Add("Account not found, try again later.", Severity.Warning);

                Return();
            }

            if (firstRender)
            {
                _loading = false;
                StateHasChanged();
            }
        }

        private async Task Update()
        {
            if (_account.Empty)
                return;

            try
            {
                _loading = true;

                StateHasChanged();

                Logger.Information("Updating account {id}", _account.Id);

                await Sender.Send(
                    new UpdateAccountCommand(
                        _account.Id,
                        _account.Username,
                        _account.Email,
                        _account.SecondaryPassword,
                        _account.DiscordId,
                        _account.AccessLevel,
                        _account.Premium,
                        _account.Silk,
                        _account.MembershipExpirationDate,
                        _account.ReceiveWelcome)
                );

                Logger.Information("Account {id} updated.", _account.Id);

                Toast.Add("Account updated.", Severity.Success);

                Nav.NavigateTo("/accounts");
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating account id {id}: {ex}", _account.Id, ex.Message);
                Toast.Add("Unable to update account, try again later.", Severity.Error);
            }
            finally
            {
                _loading = false;

                StateHasChanged();
            }
        }

        private async Task BanAccount()
        {
            try
            {
                _loading = true;
                StateHasChanged();

                Logger.Information("Banning account {id}", _account.Id);

                // Set end date to far future for permanent bans
                if (_banModel.Type == AccountBlockEnum.Permanent)
                {
                    _banModel.EndDate = DateTime.MaxValue;
                }

                await Sender.Send(
                    new BanAccountCommand(
                        _banModel.AccountId,
                        _banModel.Type,
                        _banModel.Reason,
                        _banModel.StartDate ?? DateTime.Now,
                        _banModel.EndDate ?? DateTime.Now.AddDays(7))
                );

                Logger.Information("Account {id} banned successfully.", _account.Id);
                Toast.Add("Account banned successfully.", Severity.Success);

                // Refresh the page to show updated ban status
                Nav.NavigateTo($"/accounts/update/{_account.Id}", forceLoad: true);
            }
            catch (Exception ex)
            {
                Logger.Error("Error banning account id {id}: {ex}", _account.Id, ex.Message);
                Toast.Add("Unable to ban account, try again later.", Severity.Error);
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }
        }

        private void Return()
        {
            Nav.NavigateTo("/accounts");
        }

        public class BanAccountModel
        {
            public long AccountId { get; set; }
            public AccountBlockEnum Type { get; set; } = AccountBlockEnum.Short;
            public string Reason { get; set; } = string.Empty;
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
