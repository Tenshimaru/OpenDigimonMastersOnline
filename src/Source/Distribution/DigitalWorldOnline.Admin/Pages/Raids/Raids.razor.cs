using AutoMapper;
using DigitalWorldOnline.Admin.Shared;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.Enums.Admin;
using DigitalWorldOnline.Commons.ViewModel.Mobs;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Raids
{
    public partial class Raids
    {
        [Parameter]
        public string MapId { get; set; }

        [Inject]
        public ISender Sender { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        public ILogger Logger { get; set; }

        [Inject]
        public ISnackbar Toast { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        IDialogService DialogService { get; set; }

        private MudTextField<string> _filterParameter;
        private MudTable<MobViewModel> _table;
        string _mapName;
        long _id;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (long.TryParse(MapId, out _id))
            {
                Logger.Information("Searching raids by map id {id}", _id);

                var targetMap = await Sender.Send(new GetMapByIdQuery(_id));

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

        private void Return()
        {
            Nav.NavigateTo("/maps");
        }

        private async Task<TableData<MobViewModel>> ServerReload(TableState state)
        {
            var users = await Sender.Send(
                new GetRaidsQuery(
                    _id,
                    state.Page,
                    state.PageSize,
                    state.SortLabel,
                    (SortDirectionEnum)state.SortDirection.GetHashCode(),
                    _filterParameter?.Value
                )
            );

            var pageData = Mapper.Map<IEnumerable<MobViewModel>>(users.Registers);

            return new TableData<MobViewModel>() { TotalItems = users.TotalRegisters, Items = pageData };
        }

        private void Create()
        {
            Nav.NavigateTo($"/maps/raids/create/{MapId}");
        }

        private async Task Duplicate(long id)
        {
            var parameters = new DialogParameters
            {
                { "ContentText", "Do you want to create a copy of the selected raid?" },
                { "Color", Color.Primary }
            };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = DialogService.Show<ConfirmDialog>("Duplicate ", parameters, options);

            var result = await dialog.Result;

            if (!result.Cancelled)
                Nav.NavigateTo($"/maps/raids/duplicate/{id}");
            else
                await Refresh();
        }
        
        private void Update(long id)
        {
            Nav.NavigateTo($"/maps/raids/update/{id}");
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
                Nav.NavigateTo($"/maps/raids/delete/{id}");
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
