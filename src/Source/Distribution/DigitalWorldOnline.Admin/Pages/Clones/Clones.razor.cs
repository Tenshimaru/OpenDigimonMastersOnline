using AutoMapper;
using DigitalWorldOnline.Admin.Shared;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.Enums.Admin;
using DigitalWorldOnline.Commons.ViewModel.Clones;
using DigitalWorldOnline.Commons.ViewModel.Hatchs;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Clones
{
    public partial class Clones
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
        private MudTable<CloneViewModel> _table;

        private async Task<TableData<CloneViewModel>> ServerReload(TableState state)
        {
            var clons = await Sender.Send(
                new GetClonsQuery(
                    state.Page,
                    state.PageSize,
                    state.SortLabel,
                    (SortDirectionEnum)state.SortDirection.GetHashCode(),
                    _filterParameter?.Value
                )
            );

            var pageData = Mapper.Map<IEnumerable<CloneViewModel>>(clons.Registers);

            return new TableData<CloneViewModel>() { TotalItems = clons.TotalRegisters, Items = pageData };
        }

        private void Create()
        {
            Nav.NavigateTo($"/clones/create");
        }

        private void Update(long id)
        {
            Nav.NavigateTo($"/clones/update/{id}");
        }

        private async Task Delete(long id)
        {
            var parameters = new DialogParameters
            {
                { "ContentText", "Chrysalimon gonna use Data Crusher and wipe this data. Do you want to proceed? This process cannot be undone." }
            };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = DialogService.Show<ConfirmDialog>("Delete", parameters, options);

            var result = await dialog.Result;

            if (!result.Cancelled)
                Nav.NavigateTo($"/clones/delete/{id}");
            else
                await Refresh();
        }

        private async Task Refresh()
        {
            await _table.ReloadServerData();
            await new Task(() => { _table.Loading = false; });
        }
    }
}
