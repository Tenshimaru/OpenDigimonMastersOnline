using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Events.Maps.Raids
{
    public partial class RaidDuplicate
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
                Logger.Information("Invalid raid id parameter: {parameter}", MobId);
                Toast.Add("Raid not found, try again later.", Severity.Warning);

                Return();
            }

            if (firstRender)
            {
                await Duplicate();
            }
        }

        private async Task Duplicate()
        {
            try
            {
                _loading = true;
                StateHasChanged();

                Logger.Information("Duplicating raid {id}", _id);

                await Sender.Send(new DuplicateEventMobCommand(_id));

                Logger.Information("Raid {id} duplicated.", _id);

                Toast.Add("Raid duplicated.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error duplicating raid id {id}: {ex}", _id, ex.Message);
                Toast.Add("Unable to duplicate raid, try again later.", Severity.Error);
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
                Nav.NavigateTo($"/events/{_eventId}/maps/{_mapId}/raids");
            else
                Nav.NavigateTo($"/events/{_eventId}/maps");
        }
    }
}
