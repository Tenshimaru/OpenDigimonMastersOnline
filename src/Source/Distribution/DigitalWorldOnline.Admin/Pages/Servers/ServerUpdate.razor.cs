using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.ViewModel.Server;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Servers
{
    public partial class ServerUpdate
    {
        ServerUpdateViewModel _server = new();
        bool _loading = true;
        long _id;

        [Parameter]
        public string ServerId { get; set; }

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

            if (long.TryParse(ServerId, out _id))
            {
                Logger.Information("Searching server by id {id}", _id);

                var target = await Sender.Send(
                    new GetServerByIdQuery(_id)
                );

                _server = target.Register != null ?
                    new ServerUpdateViewModel(
                        target.Register.Id,
                        target.Register.Name,
                        target.Register.Experience,
                        target.Register.Maintenance,
                        target.Register.Type,
                        target.Register.Port)
                    : null;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0 || _server == null)
            {
                Logger.Information("Invalid server id parameter: {parameter}", ServerId);
                Toast.Add("Server not found, try again later.", Severity.Warning);

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
            if (_server.Empty)
                return;

            try
            {
                _loading = true;

                StateHasChanged();

                Logger.Information("Updating server {userid}", _server.Id);

                await Sender.Send(
                    new UpdateServerCommand(
                        _server.Id,
                        _server.Name,
                        _server.Experience,
                        _server.Maintenance,
                        _server.Type,
                        _server.Port)
                );

                Logger.Information("Server {id} updated.", _server.Id);

                Toast.Add("Server updated.", Severity.Success);

                Nav.NavigateTo("/servers");
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating server id {id}: {ex}", _server.Id, ex.Message);
                Toast.Add("Unable to update server, try again later.", Severity.Error);
            }
            finally
            {
                _loading = false;

                StateHasChanged();
            }
        }

        private void Return()
        {
            Nav.NavigateTo("/servers");
        }
    }
}
