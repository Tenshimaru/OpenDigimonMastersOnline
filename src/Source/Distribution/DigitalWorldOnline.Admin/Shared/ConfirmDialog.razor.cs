using Microsoft.AspNetCore.Components;
using MudBlazor;
namespace DigitalWorldOnline.Admin.Shared
{
    public partial class ConfirmDialog
    {
        [CascadingParameter] 
        MudDialogInstance MudDialog { get; set; }

        [Parameter] 
        public string ContentText { get; set; }

        [Parameter]
        public Color Color { get; set; } = Color.Error;

        void Submit() => MudDialog.Close(DialogResult.Ok(true));
        void Cancel() => MudDialog.Cancel();
    }
}
