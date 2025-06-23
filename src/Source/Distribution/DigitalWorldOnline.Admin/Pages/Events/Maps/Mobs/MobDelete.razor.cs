using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Events.Maps.Mobs
{
    public partial class MobDelete
    {
        bool _loading;
        long _id;
        long _eventId;
        long _mapId;

        [Parameter]
        public string EventId { get; set; }

        [Parameter]
        public string MapId { get; set; }

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

            long.TryParse(EventId, out _eventId);
            
            long.TryParse(MapId, out _mapId);
            
            if (long.TryParse(MobId, out _id))
            {
                var result = await Sender.Send(new GetEventMobByIdQuery(_id));

                if (result.Register != null)
                    _mapId = result.Register.EventMapConfigId;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid mob id parameter: {parameter}", MobId);
                Toast.Add("Mob not found, try again later.", Severity.Warning);

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

                Logger.Information("Deleting mob {id}", _id);

                await Sender.Send(new DeleteEventMobCommand(_id));

                Logger.Information("Mob {id} deleted.", _id);

                Toast.Add("Mob deleted.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error deleting mob id {id}: {ex}", _id, ex.Message);
                Toast.Add("Unable to delete mob, try again later.", Severity.Error);
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
                Nav.NavigateTo($"/events/{_eventId}/maps/{_mapId}/mobs");
            else
                Nav.NavigateTo($"/events/{_eventId}/maps");
        }
    }
}
