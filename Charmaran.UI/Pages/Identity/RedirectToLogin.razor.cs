using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Charmaran.UI.Pages.Identity
{
    public partial class RedirectToLogin
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = null!;
		
        private bool NotAuthorized { get; set; } = false;
		
        protected override async Task OnInitializedAsync()
        {
            AuthenticationState authState = await this.AuthenticationState;

            if (authState.User.Identity is null || authState.User.Identity.IsAuthenticated == false)
            {
                this.NavigationManager.NavigateTo("/Login");
            }
            else
            {
                this.NotAuthorized = true;
            }
        }
    }
}