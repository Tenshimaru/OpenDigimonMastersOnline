using AutoMapper;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
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
    public partial class CloneUpdate
    {
        CloneViewModel _clone = new CloneViewModel();
        bool Loading = false;
        long _id;

        [Parameter]
        public string CloneId { get; set; }

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

            if (long.TryParse(CloneId, out _id))
            {
                Logger.Information("Searching clone config by id {id}", _id);

                var target = await Sender.Send(
                    new GetClonByIdQuery(_id)
                );

                if (target.Register == null)
                    _id = 0;
                else
                {
                    _clone = Mapper.Map<CloneViewModel>(target.Register);
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid clone config id parameter: {parameter}", CloneId);
                Toast.Add("Clone config not found, try again later.", Severity.Warning);

                Return();
            }

            StateHasChanged();
        }

        private async Task Update()
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

                Logger.Information("Updating clone config id {id}", _clone.Id);

                var configDto = Mapper.Map<CloneConfigDTO>(_clone);

                await Sender.Send(new UpdateCloneConfigCommand(configDto));

                Logger.Information("Clone config id {id} update", _clone.Id);

                Toast.Add("Clone config updated successfully.", Severity.Success);

                Return();
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating clone config with id {id}: {ex}", _clone.Id, ex.Message);
                Toast.Add("Unable to update clone config, try again later.", Severity.Error);
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
