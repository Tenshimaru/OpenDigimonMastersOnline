using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.ViewModel.Asset;
using DigitalWorldOnline.Commons.ViewModel.Containers;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Containers
{
    public partial class ContainerCreation
    {
        private MudAutocomplete<ItemAssetViewModel> _selectedItemAsset;

        ContainerViewModel _container = new ContainerViewModel();

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
            if (_container.Rewards.Any())
                _container.Rewards.AddFirst(ContainerRewardViewModel.Create(_container.Rewards.Max(x => x.Id)));
            else
                _container.Rewards.AddFirst(new ContainerRewardViewModel());

            StateHasChanged();
        }

        private void DeleteReward(long id)
        {
            _container.Rewards.RemoveWhere(x => x.Id == id);

            StateHasChanged();
        }

        private async Task Create()
        {
            try
            {
                if (_container.Invalid || _container.Rewards.Any(x => x.Invalid))
                {
                    Toast.Add("Invalid container configuration.", Severity.Warning);

                    return;
                }

                Loading = true;

                StateHasChanged();

                Logger.Information("Creating new container config for item {itemId}.", _container.ItemId);

                _container.Rewards.RemoveWhere(x => x.ItemInfo == null);

                foreach (var reward in _container.Rewards)
                {
                    reward.Id = 0;
                    reward.ItemId = reward.ItemInfo.ItemId;
                    reward.ItemName = reward.ItemInfo.Name;
                }

                _container.ItemId = _container.ItemInfo.ItemId;
                _container.ItemName = _container.ItemInfo.Name;

                var newContainer = Mapper.Map<ContainerAssetDTO>(_container);

                await Sender.Send(new CreateContainerConfigCommand(newContainer));

                Logger.Information("Container config created for item {itemid}.", _container.ItemId);
                Toast.Add("Container config created successfully.", Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating container config for item {itemid}: {ex}", _container.ItemId, ex.Message);
                Toast.Add("Unable to create container config, try again later.", Severity.Error);

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
            Nav.NavigateTo($"/containers");
        }
    }
}
