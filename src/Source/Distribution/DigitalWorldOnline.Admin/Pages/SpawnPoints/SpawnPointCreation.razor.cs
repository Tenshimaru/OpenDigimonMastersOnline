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
    public partial class SpawnPointCreation
    {
        SpawnPointCreationViewModel _spawnPoint = new SpawnPointCreationViewModel();

        bool Loading = false;
        string _mapName;
        long _id;
        int _mapId;

        [Parameter]
        public string MapId { get; set; }

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

            if (long.TryParse(MapId, out _id))
            {
                Logger.Information("Searching map with id {id}", _id);

                var targetMap = await Sender.Send(
                    new GetMapByIdQuery(_id)
                );

                if (targetMap.Register == null)
                    _id = 0;
                else
                {
                    _mapName = targetMap.Register.Name;
                    _mapId = targetMap.Register.MapId;
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid map id parameter: {parameter}", MapId);
                Toast.Add("Map not found, try again later.", Severity.Warning);

                Return();
            }
        }

        private async Task Create()
        {
            if (_spawnPoint.Invalid)
                return;

            try
            {
                Loading = true;

                StateHasChanged();

                Logger.Information("Creating new spawn point with name {name}", _spawnPoint.Name);

                var newSpawnPoint = Mapper.Map<MapRegionAssetDTO>(_spawnPoint);

                await Sender.Send(new CreateSpawnPointCommand(newSpawnPoint, _mapId));

                Logger.Information("Spawn point created with name {name}", _spawnPoint.Name);

                Toast.Add("Spawn point created successfully.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating spawn point with name {name}: {ex}", _spawnPoint.Name, ex.Message);
                Toast.Add("Unable to create spawn point, try again later.", Severity.Error);
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
                Nav.NavigateTo($"/maps/spawnpoints/{MapId}");
            else
                Nav.NavigateTo($"/maps");
        }
    }
}
