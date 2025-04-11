using System.Threading.Tasks;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.AttendanceEntry;
using Charmaran.UI.Models;
using Refit;

namespace Charmaran.UI.Contracts.Refit
{
    public interface IAttendanceEntryApiService
    {
        [Get("/api/attendanceentry")]
        public Task<ApiResponse<GetEmployeeAttendanceEntriesResponse>> GetAttendanceEntries(int employeeId, int year);

        [Post("/api/attendanceentry")]
        public Task<ApiResponse<CreateAttendanceEntryResponse>> AddAttendanceEntry([Body] AttendanceEntryModel entry);

        [Delete("/api/attendanceentry")]
        public Task<ApiResponse<DeleteAttendanceEntryResponse>> DeleteAttendanceEntry(int id);

        [Put("/api/attendanceentry")]
        public Task<ApiResponse<UpdateAttendanceEntryResponse>> UpdateAttendanceEntry([Body] AttendanceEntryDto entry);
    }
}