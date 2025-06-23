using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.DTOs.Config;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.ViewModel.Asset;
using DigitalWorldOnline.Commons.ViewModel.Events;

namespace DigitalWorldOnline.Admin.Pages.Events.Maps
{
    public partial class EventMapUpdate
    {
        EventMapViewModel _map = new EventMapViewModel();
        private MudAutocomplete<MapConfigViewModel> _selectedMap;
        bool Loading = false;
        long _id;
        long _eventId;

        [Parameter]
        public string EventId { get; set; }

        [Parameter]
        public string MapId { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public ISender Sender { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        public ISnackbar Toast { get; set; }

        [Inject]
        public ILogger Logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            long.TryParse(EventId, out _eventId);
                
            if (long.TryParse(MapId, out _id))
            {
                Logger.Information("Searching event map by id {id}", _id);

                var target = await Sender.Send(
                    new GetEventMapByIdQuery(_id)
                );

                if (target.Register == null)
                    _id = 0;
                else
                {
                    _map = Mapper.Map<EventMapViewModel>(target.Register);
                }
            }
        }
        
        private async Task<IEnumerable<MapConfigViewModel>> GetMapAssets(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3)
            {
                if (string.IsNullOrEmpty(value))
                {
                    _selectedMap.Clear();
                }

                return Array.Empty<MapConfigViewModel>();
            }

            var assets = await Sender.Send(new GetMapConfigQuery(value));

            return Mapper.Map<List<MapConfigViewModel>>(assets.Registers);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid event map id parameter: {parameter}", EventId);
                Toast.Add("Event config not found, try again later.", Severity.Warning);

                Return();
            }

            StateHasChanged();
        }

        private async Task Update()
        {
            try
            {
                Loading = true;

                StateHasChanged();

                Logger.Information("Updating event map id {id}", _map.Id);

                var configDto = Mapper.Map<EventMapsConfigDTO>(_map);

                await Sender.Send(new UpdateEventMapConfigCommand(configDto));

                Logger.Information("Event map id {id} update", _map.Id);

                Toast.Add("Event map updated successfully.", Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating event map with id {id}: {ex}", _map.Id, ex.Message);
                Toast.Add("Unable to update event map, try again later.", Severity.Error);
                Return();
            }
            finally
            {
                Loading = false;

                StateHasChanged();
            }
        }

        private void Return()
        {
            Nav.NavigateTo($"/events/{_eventId}/maps");
        }
    }
}
