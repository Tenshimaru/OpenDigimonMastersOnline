using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.ViewModel.Asset;
using DigitalWorldOnline.Commons.ViewModel.Mobs;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.ViewModel.Events;

namespace DigitalWorldOnline.Admin.Pages.Events.Maps.Mobs
{
    public partial class MobCreation
    {
        private MudAutocomplete<EventMobAssetViewModel> _selectedMobAsset;
        private MudAutocomplete<ItemAssetViewModel> _selectedItemAsset;

        EventMobCreationViewModel _mob = new EventMobCreationViewModel();
        bool Loading = false;
        string _mapName;
        long _eventId;
        long _mapId;

        [Parameter]
        public string EventId { get; set; }

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

            long.TryParse(EventId, out _eventId);
            
            if (long.TryParse(MapId, out _mapId))
            {
                Logger.Information("Searching mobs by map id {id}", _mapId);

                var targetMap = await Sender.Send(
                    new GetEventMapByIdQuery(_mapId)
                );

                if (targetMap.Register == null)
                    _mapId = 0;
                else
                    _mapName = targetMap.Register.Map.Name;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_mapId == 0)
            {
                Logger.Information("Invalid map id parameter: {parameter}", MapId);
                Toast.Add("Map not found, try again later.", Severity.Warning);

                Return();
            }
        }

        private async Task<IEnumerable<EventMobAssetViewModel>> GetMobAssets(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3)
            {
                if (string.IsNullOrEmpty(value))
                {
                    _selectedMobAsset.Clear();
                }

                return Array.Empty<EventMobAssetViewModel>();
            }

            var assets = await Sender.Send(new GetMobAssetQuery(value));

            return Mapper.Map<List<EventMobAssetViewModel>>(assets.Registers);
        }

        private async Task<IEnumerable<ItemAssetViewModel>> GetItemAssets(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3)
            {
                if (string.IsNullOrEmpty(value))
                {
                    _selectedItemAsset.Clear();
                }

                return Array.Empty<ItemAssetViewModel>();
            }

            var assets = await Sender.Send(new GetItemAssetQuery(value));

            return Mapper.Map<List<ItemAssetViewModel>>(assets.Registers);
        }

        private void UpdateMobFields()
        {
            if (_selectedMobAsset.Value != null)
            {
                var backupExp = _mob.ExpReward;
                var backupLocation = _mob.Location;
                var backupDrop = _mob.DropReward;
                var backupSpawn = _mob.RespawnInterval;
                var backupDuration = _mob.Duration;
                var backupRound = _mob.Round;
                _mob = Mapper.Map<EventMobCreationViewModel>(_selectedMobAsset.Value);
                _mob.ExpReward = backupExp;
                _mob.Location = backupLocation;
                _mob.DropReward = backupDrop;
                _mob.RespawnInterval = backupSpawn > 5 ? backupSpawn : 5;
                _mob.Class = 4;
                _mob.Duration = backupDuration;
                _mob.Round = backupRound;
            }

            StateHasChanged();
        }

        private void AddDrop()
        {
            if(_mob.DropReward.Drops.Any())
                _mob.DropReward.Drops.Add(new MobItemDropViewModel(_mob.DropReward.Drops.Max(x => x.Id)));
            else
                _mob.DropReward.Drops.Add(new MobItemDropViewModel());

            StateHasChanged();
        }

        private void DeleteDrop(long id)
        {
            _mob.DropReward.Drops.RemoveAll(x => x.Id == id);

            StateHasChanged();
        }

        private async Task Create()
        {
            if (_mob.Empty)
                return;

            try
            {
                Loading = true;

                StateHasChanged();

                Logger.Information("Creating new mob with type {type}", _mob.Type);

                _mob.DropReward.Drops.RemoveAll(x => x.ItemInfo == null);

                _mob.DropReward.Drops.ForEach(drop =>
                {
                    drop.ItemId = drop.ItemInfo.ItemId;
                });

                var newMob = Mapper.Map<EventMobConfigDTO>(_mob);
                newMob.EventMapConfigId = _mapId;

                await Sender.Send(new CreateEventMobCommand(newMob));

                Logger.Information("Mob created with type {type}", _mob.Type);

                Toast.Add("Mob created successfully.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating mob with type {type}: {ex}", _mob.Type, ex.Message);
                Toast.Add("Unable to create mob, try again later.", Severity.Error);
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
            if (_mapId > 0)
                Nav.NavigateTo($"/events/{_eventId}/maps/{_mapId}/mobs");
            else
                Nav.NavigateTo($"/events/{_eventId}/maps");
        }
    }
}
