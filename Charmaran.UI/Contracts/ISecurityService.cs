using System.Threading.Tasks;
using Charmaran.UI.Identity.Models;

namespace Charmaran.UI.Contracts
{
    public interface ISecurityService
    {
        /// <summary>
        /// Login service.
        /// </summary>
        /// <param name="username">User's username.</param>
        /// <param name="password">User's password.</param>
        /// <returns>The result of the request serialized to <see cref="AuthResult"/>.</returns>
        public Task<AuthResult> LoginAsync(string username, string password);

        /// <summary>
        /// Log out the logged in user.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        public Task LogoutAsync();

        public Task<bool> CheckAuthenticatedAsync();
    }
}