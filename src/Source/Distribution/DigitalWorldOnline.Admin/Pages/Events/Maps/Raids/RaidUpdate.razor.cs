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

namespace DigitalWorldOnline.Admin.Pages.Events.Maps.Raids
{
    public partial class RaidUpdate
    {
        private MudAutocomplete<EventMobAssetViewModel> _selectedMobAsset;
        private MudAutocomplete<ItemAssetViewModel> _selectedItemAsset;

        EventMobUpdateViewModel _mob = new EventMobUpdateViewModel();
        bool Loading = false;
        long _eventId;
        long _mapId;
        long _id;

        [Parameter]
        public string EventId { get; set; }

        [Parameter]
        public string MapId { get; set; }

        [Parameter]
        public string MobId { get; set; }

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
            
            long.TryParse(MapId, out _mapId);
            if (long.TryParse(MobId, out _id))
            {
                Logger.Information("Searching raid by id {id}", _id);

                var target = await Sender.Send(
                    new GetEventMobByIdQuery(_id)
                );

                if (target.Register == null)
                    _id = 0;
                else
                {
                    _mapId = target.Register.EventMapConfigId;
                    _mob = Mapper.Map<EventMobUpdateViewModel>(target.Register);
                    _mob.DropReward?.Drops.ForEach(async drop =>
                    {
                        var itemInfoQuery = await Sender.Send(new GetItemAssetByIdQuery(drop.ItemId));
                        drop.ItemInfo = Mapper.Map<ItemAssetViewModel>(itemInfoQuery.Register);
                    });
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid raid id parameter: {parameter}", MobId);
                Toast.Add("Raid not found, try again later.", Severity.Warning);

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

                return new EventMobAssetViewModel[0];
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

                return new ItemAssetViewModel[0];
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
                var backupMapId = _mob.GameMapConfigId;
                var backupId = _mob.Id;
                var backupRespawn = _mob.RespawnInterval;
                var backupDuration = _mob.Duration;
                var backupRound = _mob.Round;

                _mob = Mapper.Map<EventMobUpdateViewModel>(_selectedMobAsset.Value);
                _mob.ExpReward = backupExp;
                _mob.Location = backupLocation;
                _mob.DropReward = backupDrop;
                _mob.GameMapConfigId = backupMapId;
                _mob.Id = backupId;
                _mob.RespawnInterval = backupRespawn;
                _mob.Class = 8;
                _mob.Duration = backupDuration;
                _mob.Round = backupRound;
            }

            StateHasChanged();
        }

        private void AddDrop()
        {
            if (_mob.DropReward.Drops.Any())
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

        private async Task Update()
        {
            try
            {
                Loading = true;

                StateHasChanged();

                Logger.Information("Updating raid id {id}", _mob.Id);

                _mob.DropReward.Drops.RemoveAll(x => x.ItemInfo == null);
                _mob.DropReward.Drops.ForEach(drop =>
                {
                    drop.ItemId = drop.ItemInfo.ItemId;
                });

                var newMob = Mapper.Map<EventMobConfigDTO>(_mob);
                newMob.Id = 0;
                newMob.Location.Id = 0;
                newMob.ExpReward.Id = 0;
                newMob.DropReward.Id = 0;
                newMob.DropReward.BitsDrop.Id = 0;
                newMob.DropReward.Drops.ForEach(drop => { drop.Id = 0; });

                await Sender.Send(new CreateEventMobCommand(newMob));

                await Sender.Send(new DeleteEventMobCommand(_mob.Id));

                Logger.Information("Raid id {id} update", _mob.Id);

                Toast.Add("Raid updated successfully.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating raid with id {id}: {ex}", _mob.Id, ex.Message);
                Toast.Add("Unable to update raid, try again later.", Severity.Error);
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
                Nav.NavigateTo($"/events/{_eventId}/maps/{_mapId}/raids");
            else
                Nav.NavigateTo($"/events/{_eventId}/maps");
        }
    }
}
