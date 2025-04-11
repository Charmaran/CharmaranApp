using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Charmaran.UI.Contracts.Identity;
using Charmaran.UI.Identity.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Charmaran.UI.Identity
{
    public class SecurityService : ISecurityService
    {
        private readonly CookieAuthenticationStateProvider _authenticationStateProvider;
        private readonly HttpClient _httpClient;

        public SecurityService(AuthenticationStateProvider authenticationStateProvider, IHttpClientFactory httpClientFactory)
        {
            this._authenticationStateProvider = (CookieAuthenticationStateProvider)authenticationStateProvider;
            this._httpClient = httpClientFactory.CreateClient("Auth");
        }
        
        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            try
            {
                HttpResponseMessage result = await this._httpClient.PostAsJsonAsync(
                    "/login?useCookies=true", new
                    {
                        Email = username,
                        Password = password
                    });

                if (result.IsSuccessStatusCode)
                {
                    this._authenticationStateProvider.NotifyUserAuthentication();
                    return new AuthResult { Success = true };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // ignored 
            }

            return new AuthResult
            {
                Success = false,
                Message = "Invalid username and/or password."
            };
        }

        public async Task LogoutAsync()
        {
            const string empty = "{}";
            StringContent emptyContent = new StringContent(empty, Encoding.UTF8, "application/json");
            await this._httpClient.PostAsync("logout", emptyContent);
            
            this._authenticationStateProvider.NotifyUserLogout();
        }

        public async Task<bool> CheckAuthenticatedAsync()
        {
            AuthenticationState result = await this._authenticationStateProvider.GetAuthenticationStateAsync();
            return result.User.Identity?.IsAuthenticated ?? false;
        }
    }
}