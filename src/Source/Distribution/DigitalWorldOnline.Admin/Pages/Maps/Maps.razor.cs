using AutoMapper;
using DigitalWorldOnline.Admin.Shared;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.Enums.Admin;
using DigitalWorldOnline.Commons.ViewModel.Maps;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Maps
{
    public partial class Maps
    {
        [Inject]
        public ISender Sender { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        IDialogService DialogService { get; set; }

        private MudTextField<string> _filterParameter;
        private MudTable<MapViewModel> _table;

        private async Task<TableData<MapViewModel>> ServerReload(TableState state)
        {
            var users = await Sender.Send(
                new GetMapsQuery(
                    state.Page,
                    state.PageSize,
                    state.SortLabel,
                    (SortDirectionEnum)state.SortDirection.GetHashCode(),
                    _filterParameter?.Value
                )
            );

            var pageData = Mapper.Map<IEnumerable<MapViewModel>>(users.Registers);

            return new TableData<MapViewModel>() { TotalItems = users.TotalRegisters, Items = pageData };
        }

        private void ViewMobs(long id)
        {
            Nav.NavigateTo($"/maps/mobs/{id}");
        }
        
        private void ViewRaids(long id)
        {
            Nav.NavigateTo($"/maps/raids/{id}");
        }
        
        private void ViewSpawnPoints(long id)
        {
            Nav.NavigateTo($"/maps/spawnpoints/{id}");
        }

        private async Task Reset(long id)
        {
            var parameters = new DialogParameters
            {
                { "ContentText", "All the related config for this map gonna be reseted. Do you want to proceed? This process cannot be undone." }
            };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = DialogService.Show<ConfirmDialog>("Reset", parameters, options);

            var result = await dialog.Result;

            if (!result.Cancelled)
                Nav.NavigateTo($"/maps/reset/{id}");
            else
                await Refresh();
        }

        private async Task Filter()
        {
            await Refresh();
        }

        private async Task Clear()
        {
            _filterParameter.Clear();

            await Refresh();
        }

        private async Task Refresh()
        {
            await _table.ReloadServerData();
            await new Task(() => { _table.Loading = false; });
        }
    }
}
