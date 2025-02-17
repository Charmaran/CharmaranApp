using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Charmaran.Domain.Entities;
using Charmaran.Shared.Identity;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Charmaran.FastEndpoints.Endpoints.Identity
{
    /// <summary>
    /// Endpoint to get all claims for the current user
    /// </summary>
    public class ClaimsEndpoint : EndpointWithoutRequest
    {
        private readonly ILogger<ClaimsEndpoint> _logger;
        private readonly UserManager<CharmaranUser> _userManager;

        /// <summary>
        /// Constructor for <see cref="ClaimsEndpoint"/>
        /// </summary>
        /// <param name="logger">The logger to use for logging information and errors.</param>
        /// <param name="userManager">The <see cref="UserManager{TUser}"/> to use for getting claims.</param>
        public ClaimsEndpoint(ILogger<ClaimsEndpoint> logger, UserManager<CharmaranUser> userManager)
        {
            this._logger = logger;
            this._userManager = userManager;
        }
        
        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        public override void Configure()
        {
            this.Get("/claims");
        }

        /// <summary>
        /// Handles an incoming request to retrieve all claims for the current user.
        /// </summary>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        public override async Task HandleAsync(CancellationToken ct)
        {
            Dictionary<string, string> claims = new Dictionary<string, string>();
            try
            {
                ClaimsIdentity? identity = User.Identity as ClaimsIdentity;
                if (identity == null)
                {
                    await this.SendUnauthorizedAsync(ct);
                    return;
                }
                
                CharmaranUser? user = await this._userManager.GetUserAsync(User);
                
                if (user == null)
                {
                    await this.SendUnauthorizedAsync(ct);
                    return;
                }

                claims.Add(CustomClaims._firstName, user.FirstName);
                claims.Add(CustomClaims._lastName, user.LastName);
                claims.Add(ClaimTypes.Name, user.UserName!);
                claims.Add(ClaimTypes.Email, user.Email!);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error getting claims");
                await this.SendAsync("Unexpected error occurred", 500, cancellation: ct);
            }
            
            await this.SendAsync(claims, 200, cancellation: ct);
        }
    }
}