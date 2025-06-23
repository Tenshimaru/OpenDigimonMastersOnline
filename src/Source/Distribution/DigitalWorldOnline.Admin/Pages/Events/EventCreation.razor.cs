using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.ViewModel.Events;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;
using DigitalWorldOnline.Commons.DTOs.Config.Events;

namespace DigitalWorldOnline.Admin.Pages.Events
{
    public partial class EventCreation
    {
        EventViewModel _event = new EventViewModel();

        bool Loading = false;

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

        private async Task Create()
        {
            try
            {
                Loading = true;

                StateHasChanged();

                Logger.Information("Creating new _event config.");

                var newConfig = Mapper.Map<EventConfigDTO>(_event);

                await Sender.Send(new CreateEventConfigCommand(newConfig));

                Logger.Information("Event config created.");
                Toast.Add("Event config created successfully.", Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating hatch config: {ex}", ex.Message);
                Toast.Add("Unable to create hatch config, try again later.", Severity.Error);

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
            Nav.NavigateTo($"/events");
        }
    }
}
