using System.Threading.Tasks;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Requests.AttendanceEntry;
using Charmaran.Shared.AttendanceTracker.Responses.AttendanceEntry;
using Charmaran.UI.Models;
using Refit;

namespace Charmaran.UI.Contracts.Refit
{
    public interface IAttendanceEntryApiService
    {
        [Get("/api/attendanceentry")]
        public Task<ApiResponse<GetEmployeeAttendanceEntriesResponse>> GetAttendanceEntries(GetEmployeeAttendanceEntriesApiRequest req);

        [Post("/api/attendanceentry")]
        public Task<ApiResponse<CreateAttendanceEntryResponse>> AddAttendanceEntry(CreateAttendanceEntryApiRequest req);

        [Delete("/api/attendanceentry")]
        public Task<ApiResponse<DeleteAttendanceEntryResponse>> DeleteAttendanceEntry(DeleteAttendanceEntryApiRequest req);

        [Put("/api/attendanceentry")]
        public Task<ApiResponse<UpdateAttendanceEntryResponse>> UpdateAttendanceEntry(UpdateAttendanceEntryApiRequest req);
    }
}