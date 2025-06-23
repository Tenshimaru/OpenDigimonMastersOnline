using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using DigitalWorldOnline.Admin.Services;
using DigitalWorldOnline.Admin.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace DigitalWorldOnline.Admin.Pages.Downloads
{
    public partial class Downloads
    {
        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public IJSRuntime Js { get; set; }

        [Inject]
        public ISnackbar Toast { get; set; }

        [Inject]
        public IDownloadService DownloadService { get; set; }

        [Inject]
        public ILogger<Downloads> Logger { get; set; }

        [Inject]
        public IAuditService AuditService { get; set; }

        [Inject]
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        private bool _isLoading = false;
        private IEnumerable<DownloadInfo> _availableDownloads = new List<DownloadInfo>();

        protected override async Task OnInitializedAsync()
        {
            await LoadAvailableDownloads();
        }

        private async Task LoadAvailableDownloads()
        {
            try
            {
                _availableDownloads = DownloadService.GetAvailableDownloads();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading available downloads");
                Toast.Add("Error loading downloads", Severity.Error);
            }
        }

        private async Task DownloadX64()
        {
            await DownloadFile("DSO_Installer_x64.zip", "x64");
        }

        private async Task DownloadX86()
        {
            await DownloadFile("DSO_Installer_x86.zip", "x86");
        }

        private async Task DownloadFile(string fileName, string architecture)
        {
            if (_isLoading)
                return;

            _isLoading = true;
            StateHasChanged();

            try
            {
                var username = HttpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
                var ipAddress = HttpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

                Logger.LogInformation("Download requested: {FileName}, {Architecture}, User: {User}",
                    fileName, architecture, username);

                var result = await DownloadService.GetDownloadAsync(fileName, architecture);

                if (result.Success && result.FileData != null)
                {
                    await Js.InvokeAsync<object>("downloadFile",
                        result.FileName,
                        result.MimeType,
                        Convert.ToBase64String(result.FileData));

                    Toast.Add("Thanks for downloading Digital Shinka Online!", Severity.Success);
                    Logger.LogInformation("Download completed successfully: {FileName}", fileName);

                    // Log successful download
                    await AuditService.LogDownloadAttemptAsync(username, fileName, true, ipAddress);
                }
                else
                {
                    Toast.Add($"Download failed: {result.ErrorMessage}", Severity.Error);
                    Logger.LogWarning("Download failed: {FileName}, Error: {Error}", fileName, result.ErrorMessage);

                    // Log failed download
                    await AuditService.LogDownloadAttemptAsync(username, fileName, false, ipAddress);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error during download: {FileName}", fileName);
                Toast.Add("An unexpected error occurred during download", Severity.Error);
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }


    }
}
