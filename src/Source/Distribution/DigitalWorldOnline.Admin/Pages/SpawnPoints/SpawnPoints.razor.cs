using AutoMapper;
using DigitalWorldOnline.Admin.Shared;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.Enums.Admin;
using DigitalWorldOnline.Commons.ViewModel.SpawnPoint;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.SpawnPoints
{
    public partial class SpawnPoints
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

        private MudTable<SpawnPointViewModel> _table;
        string _mapName;
        long _id;
        int _mapId;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (long.TryParse(MapId, out _id))
            {
                Logger.Information("Searching mobs by map id {id}", _id);

                var targetMap = await Sender.Send(new GetMapByIdQuery(_id));

                if (targetMap.Register == null)
                    _id = 0;
                else
                {
                    _mapName = targetMap.Register.Name;
                    _mapId = targetMap.Register.MapId;
                }
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

        private async Task<TableData<SpawnPointViewModel>> ServerReload(TableState state)
        {
            var users = await Sender.Send(
                new GetSpawnPointsQuery(
                    _mapId,
                    state.Page,
                    state.PageSize,
                    state.SortLabel,
                    (SortDirectionEnum)state.SortDirection.GetHashCode()
                )
            );

            var pageData = Mapper.Map<IEnumerable<SpawnPointViewModel>>(users.Registers);

            return new TableData<SpawnPointViewModel>() { TotalItems = users.TotalRegisters, Items = pageData };
        }

        private void Create()
        {
            Nav.NavigateTo($"/maps/spawnpoints/create/{MapId}");
        }
        
        private void Update(long id)
        {
            Nav.NavigateTo($"/maps/spawnpoints/update/{id}");
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
                Nav.NavigateTo($"/maps/spawnpoints/delete/{id}");
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
