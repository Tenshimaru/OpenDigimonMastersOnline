using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.ViewModel.Hatchs;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Hatchs
{
    public partial class HatchCreation
    {
        HatchViewModel _hatch = new HatchViewModel();

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
                if (_hatch.Invalid)
                {
                    Toast.Add("Invalid hatch configuration.", Severity.Warning);

                    return;
                }

                Loading = true;

                StateHasChanged();

                Logger.Information("Creating new _hatch config.");

                var newConfig = Mapper.Map<HatchConfigDTO>(_hatch);

                await Sender.Send(new CreateHatchConfigCommand(newConfig));

                Logger.Information("Hatch config created.");
                Toast.Add("Hatch config created successfully.", Severity.Success);

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
            Nav.NavigateTo($"/hatchs");
        }
    }
}
