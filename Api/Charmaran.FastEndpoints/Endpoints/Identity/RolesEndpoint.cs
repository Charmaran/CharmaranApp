using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.Logging;

namespace Charmaran.FastEndpoints.Endpoints.Identity
{
    /// <summary>
    /// Endpoint to get all roles for the current user
    /// </summary>
    public class RolesEndpoint : EndpointWithoutRequest
    {
        private readonly ILogger<RolesEndpoint> _logger;

        /// <summary>
        /// Constructor for <see cref="RolesEndpoint"/>
        /// </summary>
        /// <param name="logger">The logger to use for logging information and errors.</param>
        public RolesEndpoint(ILogger<RolesEndpoint> logger)
        {
            this._logger = logger;
        }
        
        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to listen to GET requests at <c>/roles</c>.
        /// </remarks>
        public override void Configure()
        {
            this.Get("/roles");
        }

        /// <summary>
        /// Handles an incoming request to retrieve all roles for the current user.
        /// </summary>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method extracts the roles from the current user's claims identity and sends them in the response.
        /// </remarks>
        public override async Task HandleAsync(CancellationToken ct)
        {
            try
            {
                ClaimsIdentity? identity = User.Identity as ClaimsIdentity;
                if (identity == null)
                {
                    await this.SendUnauthorizedAsync(ct);
                    return;
                }
            
                var roles = identity.FindAll(identity.RoleClaimType)
                    .Select(c => new
                    {
                        c.Issuer,
                        c.OriginalIssuer,
                        c.Type,
                        c.Value,
                        c.ValueType
                    });
                
                await this.SendAsync(roles, cancellation: ct);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error getting roles");
                await this.SendAsync(e.Message, 500, cancellation: ct);
            }
        }
    }
}