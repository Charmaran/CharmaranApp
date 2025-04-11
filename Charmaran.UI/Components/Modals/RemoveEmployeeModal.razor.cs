using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace Charmaran.UI.Components.Modals
{
    public partial class RemoveEmployeeModal
    {
        [CascadingParameter] private BlazoredModalInstance ModalInstance { get; set; } = null!;

        [Parameter] public string Name { get; set; } = null!;
        
        private void SubmitForm()
        {
            ModalInstance.CloseAsync(ModalResult.Ok("Employee Deleted"));
        }

        private void Cancel()
        {
            ModalInstance.CancelAsync();
        }
    }
}