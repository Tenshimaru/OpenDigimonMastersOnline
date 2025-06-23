using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Raids
{
    public partial class RaidDelete
    {
        bool _loading;
        long _id;
        long _mapId;

        [Parameter]
        public string MobId { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public ISender Sender { get; set; }

        [Inject]
        public ISnackbar Toast { get; set; }

        [Inject]
        public ILogger Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (long.TryParse(MobId, out _id))
            {
                var result = await Sender.Send(new GetMobByIdQuery(_id));

                if (result.Register != null)
                    _mapId = result.Register.GameMapConfigId;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid raid id parameter: {parameter}", MobId);
                Toast.Add("Raid not found, try again later.", Severity.Warning);

                Return();
            }

            if (firstRender)
            {
                await Delete();
            }
        }

        private async Task Delete()
        {
            try
            {
                _loading = true;
                StateHasChanged();

                Logger.Information("Deleting raid {id}", _id);

                await Sender.Send(new DeleteMobCommand(_id));

                Logger.Information("Raid {id} deleted.", _id);

                Toast.Add("Raid deleted.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error deleting raid id {id}: {ex}", _id, ex.Message);
                Toast.Add("Unable to delete raid, try again later.", Severity.Error);
            }
            finally
            {
                _loading = false;
                StateHasChanged();

                Return();
            }
        }

        private void Return()
        {
            if (_mapId > 0)
                Nav.NavigateTo($"/maps/raids/{_mapId}");
            else
                Nav.NavigateTo($"/maps");
        }
    }
}
