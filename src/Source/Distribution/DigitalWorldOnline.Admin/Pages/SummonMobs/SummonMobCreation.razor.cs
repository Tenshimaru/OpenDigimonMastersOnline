using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.ViewModel.Asset;
using DigitalWorldOnline.Commons.ViewModel.Mobs;
using DigitalWorldOnline.Commons.ViewModel.Summons;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.SummonMobs
{
    public partial class SummonMobCreation
    {
        private MudAutocomplete<SummonMobAssetViewModel> _selectedSummonMobAsset;

        private MudAutocomplete<ItemAssetViewModel> _selectedItemAsset;

        SummonMobViewModel _mob = new SummonMobViewModel();
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
                Logger.Information("Searching mobs by map id {id}", _id);

                var targetMap = await Sender.Send(
                    new GetSummonByIdQuery(_id)
                );

                if (targetMap.Register == null)
                    _id = 0;
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

        private async Task<IEnumerable<SummonMobAssetViewModel>> GetMobAssets(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3)
            {
                if (string.IsNullOrEmpty(value))
                {
                    _selectedSummonMobAsset.Clear();
                }

                return Array.Empty<SummonMobAssetViewModel>();
            }

            var assets = await Sender.Send(new GetSummonMobAssetQuery(value));

            return Mapper.Map<List<SummonMobAssetViewModel>>(assets.Registers);
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
            if (_selectedSummonMobAsset.Value != null)
            {
                var backupExp = _mob.ExpReward;
                var backupLocation = _mob.Location;
                var backupDrop = _mob.DropReward;
                var backupSpawn = _mob.Duration;

                _mob = Mapper.Map<SummonMobViewModel>(_selectedSummonMobAsset.Value);
                _mob.ExpReward = backupExp;
                _mob.Location = backupLocation;
                _mob.DropReward = backupDrop;
                _mob.Duration = backupSpawn > 5 ? backupSpawn : 5;
            }

            StateHasChanged();
        }

        private void AddDrop()
        {
            if(_mob.DropReward.Drops.Any())
                _mob.DropReward.Drops.Add(new SummonMobItemDropViewModel(_mob.DropReward.Drops.Max(x => x.Id)));
            else
                _mob.DropReward.Drops.Add(new SummonMobItemDropViewModel());

            StateHasChanged();
        }

        private void DeleteDrop(long id)
        {
            _mob.DropReward.Drops.RemoveAll(x => x.Id == id);

            StateHasChanged();
        }

        private async Task Create()
        {
            try
            {
                if (_mob == null)
                {
                    Logger.Error("Mob data is null. Cannot create summon mob.");
                    Toast.Add("Mob data is invalid.",Severity.Error);
                    return;
                }

                Loading = true;
                StateHasChanged();

                Logger.Information("Creating new mob with type {type}",_mob.Type);

                // Remove drops that have no ItemInfo
                _mob.DropReward.Drops.RemoveAll(x => x.ItemInfo == null);

                // Ensure each drop has an ItemId
                _mob.DropReward.Drops.ForEach(drop =>
                {
                    drop.ItemId = drop.ItemInfo?.ItemId ?? 0; // Avoids potential null reference
                });

                // Map to DTO
                var newMob = Mapper.Map<SummonMobDTO>(_mob);
                newMob.Id = 0; // Ensure it's treated as a new entity
                newMob.SummonDTOId = _id;

                // Send command to create
                await Sender.Send(new CreateSummonMobCommand(newMob));

                Logger.Information("Mob created successfully with type {type}",_mob.Type);
                Toast.Add("Mob created successfully.",Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating mob with type {type}: {exception}",_mob?.Type,ex.ToString());
                Toast.Add("Unable to create mob. Please try again later.",Severity.Error);
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
                Nav.NavigateTo($"/summons/SummonMobs/{_id}");
            else
                Nav.NavigateTo($"/summons");
        }
    }
}
