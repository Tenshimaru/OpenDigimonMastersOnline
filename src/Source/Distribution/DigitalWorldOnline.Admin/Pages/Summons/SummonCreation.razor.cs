using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Enums.Admin;
using DigitalWorldOnline.Commons.ViewModel.Asset;
using DigitalWorldOnline.Commons.ViewModel.Summons;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Summons
{
    public partial class SummonCreation
    {
        private MudAutocomplete<ItemAssetViewModel> _selectedItemAsset;
        private MudAutocomplete<MapConfigViewModel> _selectedMapAsset;


        SummonViewModel _summon = new SummonViewModel();

        bool Loading = false;

        [Inject] public NavigationManager Nav { get; set; }

        [Inject] public ISender Sender { get; set; }

        [Inject] public IMapper Mapper { get; set; }

        [Inject] public ISnackbar Toast { get; set; }

        [Inject] public ILogger Logger { get; set; }

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
        private async Task<IEnumerable<MapConfigViewModel>> GetMapAssets(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3)
            {
                if (string.IsNullOrEmpty(value))
                {
                    _selectedMapAsset.Clear();
                }

                return Array.Empty<MapConfigViewModel>();
            }

            int page = 0;
            int pageSize = 10;
            string sortColumn = "Name";
            SortDirectionEnum sortDirection = SortDirectionEnum.Asc;

            var query = new GetMapsQuery(page,pageSize,sortColumn,sortDirection,value);

            var result = await Sender.Send(query);

            return Mapper.Map<List<MapConfigViewModel>>(result.Registers);
        }

        private async Task Create()
        {
            try
            {
                if (_summon == null || _summon.ItemInfo == null || _summon.MapConfig == null)
                {
                    Toast.Add("Invalid summon data.",Severity.Warning);
                    return;
                }

                Loading = true;
                StateHasChanged();

                Logger.Information("Creating new summon config for item {itemId}.",_summon.ItemInfo.ItemId);

                var summonConfig = new SummonDTO
                {
                    ItemId = _summon.ItemInfo.ItemId,
                    Maps = new List<int> { _summon.MapConfig.MapId }
                };

                await Sender.Send(new CreateSummonCommand(summonConfig));

                Logger.Information("Summon config created for item {itemId}.",_summon.ItemInfo.ItemId);
                Toast.Add("Summon config created successfully.",Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating summon config for item {itemId}: {ex}",_summon.ItemInfo.ItemId,ex.Message);
                Toast.Add("Unable to create summon config, try again later.",Severity.Error);
            }
            finally
            {
                Loading = false;
                StateHasChanged();
            }
        }

        private void Return()
        {
            Nav.NavigateTo($"/summons");
        }
    }
}
