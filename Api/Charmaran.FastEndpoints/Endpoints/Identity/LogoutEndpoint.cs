using System.Threading;
using System.Threading.Tasks;
using Charmaran.Domain.Entities;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Charmaran.FastEndpoints.Endpoints.Identity
{
    /// <summary>
    /// Endpoint to log out and clear the cookie
    /// </summary>
    public class LogoutEndpoint : Endpoint<EmptyRequest>
    {
        private readonly SignInManager<CharmaranUser> _signInManager;

        /// <summary>
        /// Constructor for <see cref="LogoutEndpoint"/>.
        /// </summary>
        /// <param name="signInManager">The <see cref="SignInManager{TUser}"/> to use for signing out.</param>
        public LogoutEndpoint(SignInManager<CharmaranUser> signInManager)
        {
            this._signInManager = signInManager;
        }
        
        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to listen to POST requests at <c>/logout</c> and uses the
        /// <c>IdentityExtension</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.Post("/logout");
            this.Options(o =>
            {
                o.WithTags("IdentityExtension");
            });
        }
        
        /// <summary>
        /// Handles an incoming request to log out the user and clear the authentication cookie.
        /// </summary>
        /// <param name="req">The <see cref="EmptyRequest"/> representing the request.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method uses the <see cref="SignInManager{TUser}"/> to sign out the user and sends an OK response upon successful logout.
        /// </remarks>
        public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
        {
            await this._signInManager.SignOutAsync();
            await this.SendOkAsync(ct);
        }
    }
}