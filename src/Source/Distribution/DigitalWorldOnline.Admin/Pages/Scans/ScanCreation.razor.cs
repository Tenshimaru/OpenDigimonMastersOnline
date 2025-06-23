using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.ViewModel.Asset;
using DigitalWorldOnline.Commons.ViewModel.Clones;
using DigitalWorldOnline.Commons.ViewModel.Scans;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Scans
{
    public partial class ScanCreation
    {
        private MudAutocomplete<ItemAssetViewModel> _selectedItemAsset;

        ScanDetailViewModel _scan = new ScanDetailViewModel();

        bool Loading = false;

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

        private async Task<IEnumerable<ItemAssetViewModel>> GetItemAssets(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3)
            {
                if (string.IsNullOrEmpty(value))
                {
                    await _selectedItemAsset.Clear();
                }

                return new ItemAssetViewModel[0];
            }

            var assets = await Sender.Send(new GetItemAssetQuery(value));

            return Mapper.Map<List<ItemAssetViewModel>>(assets.Registers);
        }

        private void AddReward()
        {
            if(_scan.Rewards.Any())
                _scan.Rewards.AddFirst(ScanRewardDetailViewModel.Create(_scan.Rewards.Max(x => x.Id)));
            else
                _scan.Rewards.AddFirst(new ScanRewardDetailViewModel());

            StateHasChanged();
        }

        private void DeleteReward(long id)
        {
            _scan.Rewards.RemoveWhere(x => x.Id == id);

            StateHasChanged();
        }

        private async Task Create()
        {
            try
            {
                if(_scan.Invalid || _scan.Rewards.Any(x => x.Invalid))
                {
                    Toast.Add("Invalid scan configuration.", Severity.Warning);

                    return;
                }

                Loading = true;

                StateHasChanged();

                Logger.Information("Creating new scan config for item {itemId}.", _scan.ItemId);

                _scan.Rewards.RemoveWhere(x => x.ItemInfo == null);

                foreach (var reward in _scan.Rewards)
                {
                    reward.Id = 0;
                    reward.ItemId = reward.ItemInfo.ItemId;
                    reward.ItemName = reward.ItemInfo.Name;
                }

                _scan.ItemId = _scan.ItemInfo.ItemId;
                _scan.ItemName = _scan.ItemInfo.Name;

                var newScan = Mapper.Map<ScanDetailAssetDTO>(_scan);

                await Sender.Send(new CreateScanConfigCommand(newScan));

                Logger.Information("Scan config created for item {itemid}.", _scan.ItemId);
                Toast.Add("Scan config created successfully.", Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating scan config for item {itemid}: {ex}", _scan.ItemId, ex.Message);
                Toast.Add("Unable to create scan config, try again later.", Severity.Error);

                Return();
            }
            finally
            {
                Loading = false;

                StateHasChanged();
            }
        }

        private void Return()
        {
            Nav.NavigateTo($"/scans");
        }
    }
}
