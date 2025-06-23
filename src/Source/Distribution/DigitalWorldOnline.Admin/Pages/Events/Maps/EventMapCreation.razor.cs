using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.ViewModel.Events;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.ViewModel.Asset;

namespace DigitalWorldOnline.Admin.Pages.Events.Maps
{
    public partial class EventMapCreation
    {
        EventMapViewModel _map = new EventMapViewModel();
        private MudAutocomplete<MapConfigViewModel> _selectedMap;

        bool Loading = false;
        long _eventId;
        
        [Parameter]
        public string EventId { get; set; }
        
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

            if (long.TryParse(EventId, out _eventId))
            {
                var result = await Sender.Send(new GetEventConfigByIdQuery(_eventId));

                if (result.Register != null)
                    _eventId = result.Register.Id;
            }
        }
        
        private async Task Create()
        {
            try
            {
                Loading = true;

                StateHasChanged();

                Logger.Information("Creating new _event config.");

                var newConfig = Mapper.Map<EventMapsConfigDTO>(_map);
                newConfig.EventConfigId = _eventId;
                newConfig.MapId = _selectedMap.Value.MapId;
                
                await Sender.Send(new CreateEventMapConfigCommand(newConfig));

                Logger.Information("Event config created.");
                Toast.Add("Event config created successfully.", Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating event map : {ex}", ex.Message);
                Toast.Add("Unable to create event map , try again later.", Severity.Error);
                Return();
            }
            finally
            {
                Loading = false;

                StateHasChanged();
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
        
        private void Return()
        {
            Nav.NavigateTo($"/events/{_eventId}/maps");
        }
    }
}
