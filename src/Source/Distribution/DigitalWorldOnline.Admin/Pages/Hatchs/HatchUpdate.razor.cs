using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
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
    public partial class HatchUpdate
    {
        HatchViewModel _hatch = new HatchViewModel();
        bool Loading = false;
        long _id;

        [Parameter]
        public string HatchId { get; set; }

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

            if (long.TryParse(HatchId, out _id))
            {
                Logger.Information("Searching hatch config by id {id}", _id);

                var target = await Sender.Send(
                    new GetHatchConfigByIdQuery(_id)
                );

                if (target.Register == null)
                    _id = 0;
                else
                {
                    _hatch = Mapper.Map<HatchViewModel>(target.Register);
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid hatch config id parameter: {parameter}", HatchId);
                Toast.Add("Hatch config not found, try again later.", Severity.Warning);

                Return();
            }

            StateHasChanged();
        }

        private async Task Update()
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

                Logger.Information("Updating hatch config id {id}", _hatch.Id);

                var configDto = Mapper.Map<HatchConfigDTO>(_hatch);

                await Sender.Send(new UpdateHatchConfigCommand(configDto));

                Logger.Information("Hatch config id {id} update", _hatch.Id);

                Toast.Add("Hatch config updated successfully.", Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating hatch config with id {id}: {ex}", _hatch.Id, ex.Message);
                Toast.Add("Unable to update hatch config, try again later.", Severity.Error);
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
