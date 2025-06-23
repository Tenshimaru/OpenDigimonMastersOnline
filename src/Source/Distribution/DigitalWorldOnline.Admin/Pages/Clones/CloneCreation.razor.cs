using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.ViewModel.Clones;
using DigitalWorldOnline.Commons.ViewModel.Hatchs;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Clones
{
    public partial class CloneCreation
    {
        CloneViewModel _clone = new CloneViewModel();

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
                if (_clone.Invalid)
                {
                    Toast.Add("Invalid clone configuration.", Severity.Warning);

                    return;
                }

                Loading = true;

                StateHasChanged();

                Logger.Information("Creating new clone config.");

                var newConfig = Mapper.Map<CloneConfigDTO>(_clone);

                await Sender.Send(new CreateCloneConfigCommand(newConfig));

                Logger.Information("Clone config created.");
                Toast.Add("Clone config created successfully.", Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating clone config: {ex}", ex.Message);
                Toast.Add("Unable to create clone config, try again later.", Severity.Error);

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
            Nav.NavigateTo($"/clones");
        }
    }
}
