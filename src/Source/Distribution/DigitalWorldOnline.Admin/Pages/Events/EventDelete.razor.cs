using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Events
{
    public partial class EventDelete
    {
        bool _loading;
        long _id;

        [Parameter]
        public string EventId { get; set; }

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

            if (long.TryParse(EventId, out _id))
            {
                var result = await Sender.Send(new GetEventConfigByIdQuery(_id));

                if (result.Register != null)
                    _id = result.Register.Id;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid hatch config id parameter: {parameter}", EventId);
                Toast.Add("Event config not found, try again later.", Severity.Warning);

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

                Logger.Information("Deleting hatch config id {id}", _id);

                await Sender.Send(new DeleteEventConfigCommand(_id));

                Logger.Information("Event config id {id} deleted.", _id);

                Toast.Add("Event config deleted.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error deleting hatch config id {id}: {ex}", _id, ex.Message);
                Toast.Add("Unable to delete hatch config, try again later.", Severity.Error);
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
            Nav.NavigateTo($"/events");
        }
    }
}
