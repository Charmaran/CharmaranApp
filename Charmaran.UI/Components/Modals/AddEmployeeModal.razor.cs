using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Charmaran.Shared.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Charmaran.UI.Components.Modals
{
    public partial class AddEmployeeModal
    {
        [CascadingParameter] private BlazoredModalInstance ModalInstance { get; set; } = null!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
        private string? Name { get; set; }

        private async Task SubmitForm()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                await JsRuntime.InvokeVoidAsync("alert", "Please enter a name");
                return;
            }
            
            if (this.Name.HasFirstAndLastName() == false)
            {
                await JsRuntime.InvokeVoidAsync("alert", "Please enter a first and last name");
                return;
            }

            if (this.Name.ContainsLettersOnly() == false)
            {
                await JsRuntime.InvokeVoidAsync("alert", "Please enter a name with letters only");
                return;
            }

            await ModalInstance.CloseAsync(ModalResult.Ok(this.Name.CleanName()));

        }

        private void Cancel()
        {
            ModalInstance.CancelAsync();
        }
    }
}