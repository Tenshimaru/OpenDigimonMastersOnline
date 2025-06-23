using DigitalWorldOnline.Admin.Pages.Scans;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.Enums.Admin;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Summons
{
    public partial class SummonDelete
    {
        bool _loading;
        long _id;

        [Parameter]
        public string summonId { get; set; }

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

            if (long.TryParse(summonId,out _id))
            {
                var result = await Sender.Send(new GetSummonByIdQuery(_id));

                if (result.Register != null)
                    _id = result.Register.Id;
            }
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid scan config id parameter: {parameter}",summonId);
                Toast.Add("Scan config not found, try again later.", Severity.Warning);

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

                Logger.Information("Deleting scan config id {id}", _id);

                await Sender.Send(new DeleteSummonCommand(_id));

                Logger.Information("Scan config id {id} deleted.", _id);

                Toast.Add("Summon config deleted.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error deleting scan config id {id}: {ex}", _id, ex.Message);
                Toast.Add("Unable to delete scan config, try again later.", Severity.Error);
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
            Nav.NavigateTo($"/summons");
        }
    }
}
