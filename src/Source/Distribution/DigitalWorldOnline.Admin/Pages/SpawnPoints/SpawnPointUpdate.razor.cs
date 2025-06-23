using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.ViewModel.SpawnPoint;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.SpawnPoints
{
    public partial class SpawnPointUpdate
    {
        SpawnPointUpdateViewModel _spawnPoint = new SpawnPointUpdateViewModel();

        bool Loading = false;
        string _mapName;
        long _id;
        long _mapId;

        [Parameter]
        public string SpawnPointId { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public ISender Sender { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        public ISnackbar Toast { get; set; }

        [Inject]
        public ILogger Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (long.TryParse(SpawnPointId, out _id))
            {
                Logger.Information("Searching map with id {id}", _id);

                var targetSpawnPoint = await Sender.Send(new GetSpawnPointByIdQuery(_id));

                if (targetSpawnPoint.Register == null)
                    _id = 0;
                else
                {
                    _spawnPoint = Mapper.Map<SpawnPointUpdateViewModel>(targetSpawnPoint.Register);
                    _mapName = targetSpawnPoint.MapName;
                    _mapId = targetSpawnPoint.MapId;
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid spawn point id parameter: {parameter}", SpawnPointId);
                Toast.Add("Spawn point not found, try again later.", Severity.Warning);

                Return();
            }
        }

        private async Task Update()
        {
            if (_spawnPoint.Invalid)
                return;

            try
            {
                Loading = true;

                StateHasChanged();

                Logger.Information("Updating spawn point id {id}", _spawnPoint.Id);

                var updatedSpawnPoint = Mapper.Map<MapRegionAssetDTO>(_spawnPoint);

                await Sender.Send(new UpdateSpawnPointCommand(updatedSpawnPoint, _mapId));

                Logger.Information("Spawn point id {id} updated", _spawnPoint.Id);

                Toast.Add("Spawn point updated successfully.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating spawn point id {id}: {ex}", _spawnPoint.Id, ex.Message);
                Toast.Add("Unable to update spawn point, try again later.", Severity.Error);
            }
            finally
            {
                Loading = false;

                StateHasChanged();

                Return();
            }
        }

        private void Return()
        {
            if (_id > 0)
                Nav.NavigateTo($"/maps/spawnpoints/{_mapId}");
            else
                Nav.NavigateTo($"/maps");
        }
    }
}
