using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.ViewModel;

using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;
using DigitalWorldOnline.Commons.ViewModel.Login;
using Microsoft.AspNetCore.WebUtilities;
using DigitalWorldOnline.Application.Separar.Queries;

namespace DigitalWorldOnline.Admin.Pages.Logins
{
    public partial class Login
    {
        private LoginViewModel _login = new();
        private bool _loading = false;
        private bool _disabledButton = false;

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public ISnackbar Toast { get; set; }

        [Inject]
        public ILogger Logger { get; set; }

        [Inject]
        public ISender Sender { get; set; }

        [Inject]
        public LoginModelRepository LoginRepository { get; set; }

        [Inject]
        public DigitalWorldOnline.Admin.Services.IAuditService AuditService { get; set; }

        [Inject]
        public Microsoft.AspNetCore.Http.IHttpContextAccessor HttpContextAccessor { get; set; }

        private void Disable()
        {
            _disabledButton = true;
        }

        private async Task DoLogin()
        {
            if (_login.Empty)
            {
                _disabledButton = false;
                return;
            }

            _loading = true;

            StateHasChanged();

            try
            {
                var ipAddress = HttpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = HttpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "Unknown";

                var loginResult = await Sender.Send(new CheckPortalAccessQuery(_login.Username, _login.Password.HashPassword()));

                if (loginResult == UserAccessLevelEnum.Unauthorized)
                {
                    Logger.Information("Log-in failed for username {username}", _login.Username);
                    Toast.Add("Incorrect password or username not found.", Severity.Warning);

                    // Log failed login attempt
                    await AuditService.LogLoginAttemptAsync(_login.Username, false, ipAddress, userAgent);
                }
                else
                {
                    LoginRepository.Logins.Add(
                    new LoginViewModel()
                    {
                        Username = _login.Username,
                        AccessLevel = loginResult
                    });

                    Logger.Information("Log-in successfull for username {username}", _login.Username);

                    // Log successful login attempt
                    await AuditService.LogLoginAttemptAsync(_login.Username, true, ipAddress, userAgent);

                    if (QueryHelpers.ParseQuery(Nav.ToAbsoluteUri(Nav.Uri).Query).TryGetValue("returnUrl", out var returnUrl))
                        Nav.NavigateTo($"/do-login?username={_login.Username}&returnUrl={returnUrl}", true);
                    else
                        Nav.NavigateTo($"/do-login?username={_login.Username}", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error on log-in with username {_login.Username}.\n\nError: {ex.Message}.");
                Toast.Add("Unable to log-in, try again later.", Severity.Error);
            }
            finally
            {
                _loading = false;
                _disabledButton = false;

                StateHasChanged();
            }
        }
    }
}
