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
using DigitalWorldOnline.Commons.ViewModel.Events;

namespace DigitalWorldOnline.Admin.Pages.Events.Maps
{
    public partial class EventMaps
    {
        long _eventId;
        [Parameter] public string EventId { get; set; }

        [Inject] public ISender Sender { get; set; }

        [Inject] public IMapper Mapper { get; set; }

        [Inject] public NavigationManager Nav { get; set; }

        [Inject] IDialogService DialogService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            long.TryParse(EventId, out _eventId);
        }

        private MudTextField<string> _filterParameter;
        private MudTable<EventMapViewModel> _table;

        private async Task<TableData<EventMapViewModel>> ServerReload(TableState state)
        {
            var users = await Sender.Send(
                new GetEventMapsQuery(
                    _eventId,
                    state.Page,
                    state.PageSize,
                    state.SortLabel,
                    (SortDirectionEnum)state.SortDirection.GetHashCode(),
                    _filterParameter?.Value
                )
            );

            var pageData = Mapper.Map<IEnumerable<EventMapViewModel>>(users.Registers);

            return new TableData<EventMapViewModel>() { TotalItems = users.TotalRegisters, Items = pageData };
        }

        private void ViewMobs(long id)
        {
            Nav.NavigateTo($"/events/{_eventId}/maps/{id}/mobs");
        }

        private void ViewRaids(long id)
        {
            Nav.NavigateTo($"/events/{_eventId}/maps/{id}/raids");
        }

        private void Create()
        {
            Nav.NavigateTo($"/events/{_eventId}/maps/create");
        }

        private async Task Reset(long id)
        {
            var parameters = new DialogParameters
            {
                {
                    "ContentText",
                    "All the related config for this map gonna be reseted. Do you want to proceed? This process cannot be undone."
                }
            };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = DialogService.Show<ConfirmDialog>("Reset", parameters, options);

            var result = await dialog.Result;

            if (!result.Cancelled)
                Nav.NavigateTo($"/events/maps/reset/{id}");
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
