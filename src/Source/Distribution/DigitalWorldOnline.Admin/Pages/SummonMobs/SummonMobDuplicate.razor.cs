using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Queries;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.SummonMobs
{
    public partial class SummonMobDuplicate
    {
        bool _loading;
        long _id;
        long _mapId;

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

            if (long.TryParse(MobId, out _id))
            {
                var result = await Sender.Send(new GetSummonMobByIdQuery(_id));

                if (result.Register != null)
                    _mapId = result.Register.SummonDTOId;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_id == 0)
            {
                Logger.Information("Invalid mob id parameter: {parameter}", MobId);
                Toast.Add("Mob not found, try again later.", Severity.Warning);

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

                Logger.Information("Duplicating mob {id}", _id);

                await Sender.Send(new DuplicateSummonMobCommand(_id));

                Logger.Information("Mob {id} duplicated.", _id);

                Toast.Add("Mob duplicated.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.Error("Error duplicating mob id {id}: {ex}", _id, ex.Message);
                Toast.Add("Unable to duplicate mob, try again later.", Severity.Error);
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
                Nav.NavigateTo($"/summons/SummonMobs/{_mapId}");
            else
                Nav.NavigateTo($"/summons");
        }
    }
}
