using System.Threading.Tasks;
using Refit;

namespace Charmaran.UI.Contracts.Refit
{
    
    public interface IAuthenticationApiService
    {
        [Delete("/AttendanceEntry/{id}")]
        public Task<ApiResponse<bool>> DeleteAttendanceEntry(int id);
    }
}