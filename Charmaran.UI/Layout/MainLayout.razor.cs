using System.Linq;
using System.Threading.Tasks;
using Charmaran.Shared.Identity;
using Charmaran.UI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Charmaran.UI.Layout
{
    public partial class MainLayout
    {
        [Inject] public NavigationManager NavigationManager { get; init; } = null!;
        [Inject] public ISecurityService SecurityService { get; init; } = null!;
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
        
        private string? FullName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState authState = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
            
            string? firstName = authState.User.Claims.Where(claim => string.Equals(claim.Type, CustomClaims._firstName)).Select(c => c.Value).FirstOrDefault();
            string? lastName = authState.User.Claims.Where(claim => string.Equals(claim.Type, CustomClaims._lastName)).Select(c => c.Value).FirstOrDefault();
            this.FullName = $"{firstName} {lastName}";
        }

        private void LoginButton_Click()
        {
            this.NavigationManager.NavigateTo("/Login");
        }
        
        private void LogoutButton_Click()
        {
            this.SecurityService.LogoutAsync();
            this.NavigationManager.NavigateTo("/");
        }
    }
}