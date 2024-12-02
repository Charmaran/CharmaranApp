using System.Collections.Generic;
using System.Linq;
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
            // default to not authenticated
            ClaimsPrincipal user = this._anonymous;

            try
            {
                // the user info endpoint is secured, so if the user isn't logged in this will fail
                HttpResponseMessage userResponse = await this._httpClient.GetAsync("manage/info");

                // throw if user info wasn't retrieved
                userResponse.EnsureSuccessStatusCode();

                // user is authenticated,so let's build their authenticated identity
                string userJson = await userResponse.Content.ReadAsStringAsync();
                UserInfo? userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, this._jsonSerializerOptions);

                if (userInfo != null)
                {
                    // in this example app, name and email are the same
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userInfo.Email),
                        new Claim(ClaimTypes.Email, userInfo.Email),
                    };

                    // add any additional claims
                    claims.AddRange(
                        userInfo.Claims.Where(c => c.Key != ClaimTypes.Name && c.Key != ClaimTypes.Email)
                            .Select(c => new Claim(c.Key, c.Value)));

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
    }
}