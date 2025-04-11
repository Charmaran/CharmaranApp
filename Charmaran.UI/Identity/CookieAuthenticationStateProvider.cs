using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Charmaran.UI.Identity.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Charmaran.UI.Identity
{
    public class CookieAuthenticationStateProvider : AuthenticationStateProvider
    {
        /// <summary>
        /// Default anonymous user
        /// </summary>
        private readonly ClaimsPrincipal _anonymous;
        
        /// <summary>
        /// Special auth client.
        /// </summary>
        private readonly HttpClient _httpClient;
        
        /// <summary>
        /// Map the JavaScript-formatted properties to C#-formatted classes.
        /// </summary>
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        
        public CookieAuthenticationStateProvider(IHttpClientFactory httpClientFactory)
        {
            this._anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            this._httpClient = httpClientFactory.CreateClient("Auth");
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsPrincipal user = this._anonymous;
            List<Claim> claims = new List<Claim>();

            try
            {
                // the user info endpoint is secured, so if the user isn't logged in this will fail
                HttpResponseMessage userResponse = await this._httpClient.GetAsync("manage/info");

                // throw if user info wasn't retrieved
                userResponse.EnsureSuccessStatusCode();

                // user is authenticated, so let's build their authenticated identity
                string userJson = await userResponse.Content.ReadAsStringAsync();
                UserInfo? userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, this._jsonSerializerOptions);
                
                if (userInfo != null)
                {
                    // get additional claims
                    HttpResponseMessage claimsResponse = await this._httpClient.GetAsync("/claims");
                    claimsResponse.EnsureSuccessStatusCode();
                    
                    string claimsJson = await claimsResponse.Content.ReadAsStringAsync();
                    
                    Dictionary<string, string>? additionalClaims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson, this._jsonSerializerOptions);
                    if (additionalClaims != null)
                    {
                        foreach (KeyValuePair<string,string> additionalClaim in additionalClaims)
                        {
                            claims.Add(new Claim(additionalClaim.Key, additionalClaim.Value));
                        }
                    }
                    
                    // request the roles endpoint for the user's roles
                    HttpResponseMessage rolesResponse = await this._httpClient.GetAsync("roles");

                    // throw if request fails
                    rolesResponse.EnsureSuccessStatusCode();

                    // read the response into a string
                    string rolesJson = await rolesResponse.Content.ReadAsStringAsync();

                    // deserialize the roles string into an array
                    RoleClaim[]? roles = JsonSerializer.Deserialize<RoleClaim[]>(rolesJson, this._jsonSerializerOptions);

                    // add any roles to the claims collection
                    if (roles?.Length > 0)
                    {
                        foreach (RoleClaim role in roles)
                        {
                            if (!string.IsNullOrEmpty(role.Type) && !string.IsNullOrEmpty(role.Value))
                            {
                                claims.Add(new Claim(role.Type, role.Value, role.ValueType, role.Issuer, role.OriginalIssuer));
                            }
                        }
                    }

                    // set the principal
                    ClaimsIdentity id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
                    user = new ClaimsPrincipal(id);
                }
            }
            catch
            {
                // ignored exception
            }

            // return the state
            return new AuthenticationState(user);
        }
        
        public void NotifyUserAuthentication()
        {
            NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
        }

        public void NotifyUserLogout()
        {
            Task<AuthenticationState> authState = Task.FromResult(new AuthenticationState(this._anonymous));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}