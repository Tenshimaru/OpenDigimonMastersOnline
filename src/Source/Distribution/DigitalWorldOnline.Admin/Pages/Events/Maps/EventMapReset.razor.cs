using DigitalWorldOnline.Application.Admin.Commands;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Events.Maps
{
    public partial class EventMapReset
    {
        bool _loading;
        long _eventId;
        long _id;
        
        [Parameter]
        public string EventId { get; set; }

        [Parameter]
        public string Id { get; set; }

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

            long.TryParse(EventId, out _eventId);
            long.TryParse(Id, out _id);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid map id parameter: {parameter}", Id);
                Toast.Add("Map not found, try again later.", Severity.Warning);

                Return();
            }

            if (firstRender)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            try
            {
                _loading = true;
                StateHasChanged();

                Logger.Information("Reseting map {id}", _id);

                await Sender.Send(new DeleteMapMobsCommand(_id));

                Logger.Information("Map {id} reseted.", _id);

                Toast.Add("Map reseted.", Severity.Success);

                Nav.NavigateTo("/events/maps");
            }
            catch (Exception ex)
            {
                Logger.Error("Error reseting map id {id}: {ex}", _id, ex.Message);
                Toast.Add("Unable to reset map, try again later.", Severity.Error);
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
            Nav.NavigateTo("/events/maps");
        }
    }
}
