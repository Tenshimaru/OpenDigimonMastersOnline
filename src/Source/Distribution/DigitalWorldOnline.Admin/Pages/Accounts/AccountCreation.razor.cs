using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Models.Account;
using DigitalWorldOnline.Commons.ViewModel.Account;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Accounts
{
    public partial class AccountCreation
    {
        bool Loading = false;

        AccountCreationViewModel _account = new AccountCreationViewModel();

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public ISender Sender { get; set; }

        [Inject]
        public ISnackbar Toast { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        public ILogger Logger { get; set; }

        private async Task Create()
        {
            if (_account.Empty)
                return;

            try
            {
                Loading = true;

                StateHasChanged();

                Logger.Information("Creating new account with username {name}", _account.Username);

                var account = AccountModel.Create(
                    _account.Username,
                    _account.Password.Encrypt(),
                    _account.Email,
                    null,
                    _account.AccessLevel,
                    _account.Premium,
                    _account.Silk);

                var accountDto = Mapper.Map<AccountDTO>(account);

                await Sender.Send(new CreateAccountCommand(accountDto));

                Logger.Information("Account created with username {name}", _account.Username);

                Toast.Add("Account created successfully.", Severity.Success);

                Nav.NavigateTo("/accounts");
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating account with username {name}: {ex}", _account.Username, ex.Message);
                Toast.Add("Unable to create account, try again later.", Severity.Error);
            }
            finally
            {
                Loading = false;

                StateHasChanged();
            }
        }

        private void Return()
        {
            Nav.NavigateTo("/accounts");
        }
    }
}
