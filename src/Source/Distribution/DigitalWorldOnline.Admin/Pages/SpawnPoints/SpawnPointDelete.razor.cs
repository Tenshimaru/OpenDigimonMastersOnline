using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.SpawnPoints
{
    public partial class SpawnPointDelete
    {
        bool _loading;
        long _id;
        long _mapId;

        [Parameter]
        public string SpawnPointId { get; set; }

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

            if (long.TryParse(SpawnPointId, out _id))
            {
                var result = await Sender.Send(new GetSpawnPointByIdQuery(_id));

                if (result.Register != null)
                    _mapId = result.MapId;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid mob id parameter: {parameter}", SpawnPointId);
                Toast.Add("Mob not found, try again later.", Severity.Warning);

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

                Logger.Information("Deleting spawn point {id}", _id);

                await Sender.Send(new DeleteSpawnPointCommand(_id));

                Logger.Information("Spawn point {id} deleted.", _id);

                Toast.Add("Spawn point deleted.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error deleting spawn point id {id}: {ex}", _id, ex.Message);
                Toast.Add("Unable to delete spawn point, try again later.", Severity.Error);
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
            if (_mapId > 0)
                Nav.NavigateTo($"/maps/spawnpoints/{_mapId}");
            else
                Nav.NavigateTo($"/maps");
        }
    }
}
