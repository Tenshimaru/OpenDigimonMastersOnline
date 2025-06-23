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
    public partial class ContainerUpdate
    {
        private MudAutocomplete<ItemAssetViewModel> _selectedItemAsset;

        ContainerViewModel _container = new ContainerViewModel();
        bool Loading = false;
        long _id;

        [Parameter]
        public string ContainerId { get; set; }

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

            if (long.TryParse(ContainerId, out _id))
            {
                Logger.Information("Searching container config by id {id}", _id);

                var target = await Sender.Send(
                    new GetContainerByIdQuery(_id)
                );

                if (target.Register == null)
                    _id = 0;
                else
                {
                    _container = Mapper.Map<ContainerViewModel>(target.Register);

                    foreach (var reward in _container.Rewards)
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
                Logger.Information("Invalid container config id parameter: {parameter}", ContainerId);
                Toast.Add("Container config not found, try again later.", Severity.Warning);

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
            if (_container.Rewards.Any())
                _container.Rewards.AddFirst(ContainerRewardViewModel.Create(_container.Rewards.Max(x => x.Id)));
            else
                _container.Rewards.AddFirst(new ContainerRewardViewModel());

            StateHasChanged();
        }

        private void DeleteReward(long id)
        {
            var remove = _container.Rewards.FirstOrDefault(x => x.Id == id);
            _container.Rewards.Remove(remove);

            StateHasChanged();
        }

        private async Task Update()
        {
            try
            {
                if (_container.Rewards.Any(x => x.Invalid))
                {
                    Toast.Add("Invalid container configuration.", Severity.Warning);

                    return;
                }

                Loading = true;

                StateHasChanged();

                Logger.Information("Updating container config id {id}", _container.Id);

                _container.Rewards.RemoveWhere(x => x.ItemInfo == null);

                foreach (var reward in _container.Rewards)
                {
                    reward.ItemId = reward.ItemInfo.ItemId;
                    reward.ItemName = reward.ItemInfo.Name;
                }

                var containerDto = Mapper.Map<ContainerAssetDTO>(_container);

                await Sender.Send(new UpdateContainerConfigCommand(containerDto));

                Logger.Information("Container config id {id} update", _container.Id);

                Toast.Add("Container config updated successfully.", Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating container config with id {id}: {ex}", _container.Id, ex.Message);
                Toast.Add("Unable to update container config, try again later.", Severity.Error);
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
