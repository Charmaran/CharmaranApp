using System.Threading.Tasks;
using Charmaran.UI.Contracts;
using Charmaran.UI.Identity.Models;

namespace Charmaran.UI.Identity
{
    public class SecurityService : ISecurityService
    {
        public Task<AuthResult> LoginAsync(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task LogoutAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CheckAuthenticatedAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}