using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.DTOs.Config;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.ViewModel.Events;

namespace DigitalWorldOnline.Admin.Pages.Events
{
    public partial class EventUpdate
    {
        EventViewModel _event = new EventViewModel();
        bool Loading = false;
        long _id;

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

            if (long.TryParse(EventId, out _id))
            {
                Logger.Information("Searching event config by id {id}", _id);

                var target = await Sender.Send(
                    new GetEventConfigByIdQuery(_id)
                );

                if (target.Register == null)
                    _id = 0;
                else
                {
                    _event = Mapper.Map<EventViewModel>(target.Register);
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid event config id parameter: {parameter}", EventId);
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

                Logger.Information("Updating event config id {id}", _event.Id);

                var configDto = Mapper.Map<EventConfigDTO>(_event);

                await Sender.Send(new UpdateEventConfigCommand(configDto));

                Logger.Information("Event config id {id} update", _event.Id);

                Toast.Add("Event config updated successfully.", Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating event config with id {id}: {ex}", _event.Id, ex.Message);
                Toast.Add("Unable to update event config, try again later.", Severity.Error);
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
