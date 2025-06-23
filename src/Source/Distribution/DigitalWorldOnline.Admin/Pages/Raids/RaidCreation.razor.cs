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

namespace DigitalWorldOnline.Admin.Pages.Raids
{
    public partial class RaidCreation
    {
        private MudAutocomplete<MobAssetViewModel> _selectedMobAsset;
        private MudAutocomplete<ItemAssetViewModel> _selectedItemAsset;

        MobCreationViewModel _mob = new MobCreationViewModel();
        bool Loading = false;
        string _mapName;
        long _id;

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
                Logger.Information("Searching raids by map id {id}", _id);

                var targetMap = await Sender.Send(
                    new GetMapByIdQuery(_id)
                );

                if (targetMap.Register == null)
                    _id = 0;
                else
                    _mapName = targetMap.Register.Name;
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

        private async Task<IEnumerable<MobAssetViewModel>> GetRaidAssets(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3)
            {
                if (string.IsNullOrEmpty(value))
                {
                    _selectedMobAsset.Clear();
                }

                return Array.Empty<MobAssetViewModel>();
            }

            var assets = await Sender.Send(new GetMobAssetQuery(value));

            return Mapper.Map<List<MobAssetViewModel>>(assets.Registers);
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

                _mob = Mapper.Map<MobCreationViewModel>(_selectedMobAsset.Value);
                _mob.ExpReward = backupExp;
                _mob.Location = backupLocation;
                _mob.DropReward = backupDrop;
                _mob.RespawnInterval = backupSpawn > 5 ? backupSpawn : 5;
                _mob.Class = 8;
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

                Logger.Information("Creating new raid with type {type}", _mob.Type);

                _mob.DropReward.Drops.RemoveAll(x => x.ItemInfo == null);

                _mob.DropReward.Drops.ForEach(drop =>
                {
                    drop.ItemId = drop.ItemInfo.ItemId;
                });

                var newMob = Mapper.Map<MobConfigDTO>(_mob);
                newMob.GameMapConfigId = _id;

                await Sender.Send(new CreateMobCommand(newMob));

                Logger.Information("Raid created with type {type}", _mob.Type);

                Toast.Add("Raid created successfully.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating raid with type {type}: {ex}", _mob.Type, ex.Message);
                Toast.Add("Unable to create raid, try again later.", Severity.Error);
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
                Nav.NavigateTo($"/maps/raids/{_id}");
            else
                Nav.NavigateTo($"/maps");
        }
    }
}
