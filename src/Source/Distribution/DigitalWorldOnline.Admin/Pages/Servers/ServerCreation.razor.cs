using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Commons.ViewModel.Server;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Servers
{
    public partial class ServerCreation
    {
        bool Loading = false;

        ServerCreationViewModel _server = new ServerCreationViewModel();

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
            if (_server.Empty)
                return;

            try
            {
                Loading = true;

                StateHasChanged();

                Logger.Information("Creating new server with name {name}", _server.Name);

                await Sender.Send(
                    new CreateServerCommand(
                       _server.Name,
                       _server.Experience,
                       _server.Type,
                       _server.Port)
                );

                Logger.Information("Server created with name {name}", _server.Name);

                Toast.Add("Server created successfully.", Severity.Success);

                Nav.NavigateTo("/servers");
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating server with name {name}: {ex}", _server.Name, ex.Message);
                Toast.Add("Unable to create server, try again later.", Severity.Error);
            }
            finally
            {
                Loading = false;

                StateHasChanged();
            }
        }

        private void Return()
        {
            Nav.NavigateTo("/servers");
        }
    }
}
