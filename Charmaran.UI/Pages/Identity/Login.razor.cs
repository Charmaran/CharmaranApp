using System.Collections.Generic;
using System.Threading.Tasks;
using Charmaran.UI.Contracts;
using Charmaran.UI.Identity.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Charmaran.UI.Pages.Identity
{
    public partial class Login
    {
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        
        [Inject] public ISecurityService SecurityService { get; set; } = null!;
        
        private bool IsProcessing { get; set; } = false;
        private string UserName { get; set; } = string.Empty;
        private string Password { get; set; } = string.Empty;
        private List<string> Errors { get; set; } = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            // If the user is already authenticated, redirect to the home page
            AuthenticationState authState = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
            
            if (authState.User.Identity?.IsAuthenticated ?? false)
            {
                this.NavigationManager.NavigateTo("/");
            }
        }

        private async Task LoginButton_Click()
        {
            Errors = new List<string>();
            IsProcessing = true;

            if (string.IsNullOrEmpty(this.UserName))
            {
                Errors.Add("Username is required.");
            }
            
            if (string.IsNullOrEmpty(this.Password))
            {
                Errors.Add("Password is required.");
            }

            //smithhe@example.com
            //smithhe@example.com
            AuthResult result = await this.SecurityService.LoginAsync(this.UserName, this.Password);
            
            if (result.Success)
            {
                this.NavigationManager.NavigateTo("/");
            }
            else
            {
                this.Errors.Add(result.Message!);
            }
            
            IsProcessing = false;
        }
    }
}