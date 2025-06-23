using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.ViewModel.Asset;
using DigitalWorldOnline.Commons.ViewModel.Scans;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.ViewModel.Clones;

namespace DigitalWorldOnline.Admin.Pages.Scans
{
    public partial class ScanUpdate
    {
        private MudAutocomplete<ItemAssetViewModel> _selectedItemAsset;

        ScanDetailViewModel _scan = new ScanDetailViewModel();
        bool Loading = false;
        long _id;

        [Parameter]
        public string ScanId { get; set; }

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

            if (long.TryParse(ScanId, out _id))
            {
                Logger.Information("Searching scan config by id {id}", _id);

                var target = await Sender.Send(
                    new GetScanByIdQuery(_id)
                );

                if (target.Register == null)
                    _id = 0;
                else
                {
                    _scan = Mapper.Map<ScanDetailViewModel>(target.Register);

                    foreach (var reward in _scan.Rewards)
                    {
                        var itemInfoQuery = await Sender.Send(new GetItemAssetByIdQuery(reward.ItemId));
                        reward.ItemInfo = Mapper.Map<ItemAssetViewModel>(itemInfoQuery.Register);
                    }
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid scan config id parameter: {parameter}", ScanId);
                Toast.Add("Scan config not found, try again later.", Severity.Warning);

                Return();
            }

            StateHasChanged();
        }

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
            if (_scan.Rewards.Any())
                _scan.Rewards.AddFirst(ScanRewardDetailViewModel.Create(_scan.Rewards.Max(x => x.Id)));
            else
                _scan.Rewards.AddFirst(new ScanRewardDetailViewModel());

            StateHasChanged();
        }

        private void DeleteReward(long id)
        {
            var remove = _scan.Rewards.FirstOrDefault(x => x.Id == id);
            _scan.Rewards.Remove(remove);

            StateHasChanged();
        }

        private async Task Update()
        {
            try
            {
                if (_scan.Rewards.Any(x => x.Invalid))
                {
                    Toast.Add("Invalid scan configuration.", Severity.Warning);

                    return;
                }

                Loading = true;

                StateHasChanged();

                Logger.Information("Updating scan config id {id}", _scan.Id);

                _scan.Rewards.RemoveWhere(x => x.ItemInfo == null);

                foreach(var reward in _scan.Rewards)
                {
                    reward.ItemId = reward.ItemInfo.ItemId;
                    reward.ItemName = reward.ItemInfo.Name;
                }

                var scanDto = Mapper.Map<ScanDetailAssetDTO>(_scan);

                await Sender.Send(new UpdateScanConfigCommand(scanDto));

                Logger.Information("Scan config id {id} update", _scan.Id);

                Toast.Add("Scan config updated successfully.", Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating scan config with id {id}: {ex}", _scan.Id, ex.Message);
                Toast.Add("Unable to update scan config, try again later.", Severity.Error);
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
