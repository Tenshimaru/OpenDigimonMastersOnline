using AutoMapper;
using DigitalWorldOnline.Admin.Shared;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.Enums.Admin;
using DigitalWorldOnline.Commons.ViewModel.Users;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace DigitalWorldOnline.Admin.Pages.Users
{
    public partial class Users
    {
        [Inject]
        public ILogger Logger { get; set; }

        [Inject]
        public ISender Sender { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        IDialogService DialogService { get; set; }

        private MudTextField<string> _filterParameter;
        private MudTable<UserViewModel> _table;

        private async Task<TableData<UserViewModel>> ServerReload(TableState state)
        {
            var users = await Sender.Send(
                new GetUsersQuery(
                    state.Page,
                    state.PageSize,
                    state.SortLabel,
                    (SortDirectionEnum)state.SortDirection.GetHashCode(),
                    _filterParameter?.Value
                )
            );

            var pageData = Mapper.Map<IEnumerable<UserViewModel>>(users.Registers);
            return new TableData<UserViewModel>() { TotalItems = users.TotalRegisters, Items = pageData };
        }

        private void Create()
        {
            Nav.NavigateTo($"/users/create");
        }

        private void Update(long id)
        {
            Nav.NavigateTo($"/users/update/{id}");
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
                Nav.NavigateTo($"/users/delete/{id}");
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
