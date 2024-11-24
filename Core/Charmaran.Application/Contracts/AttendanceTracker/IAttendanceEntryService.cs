using System.Threading.Tasks;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.AttendanceEntry;

namespace Charmaran.Application.Contracts.AttendanceTracker
{
    public interface IAttendanceEntryService
    {
        Task<CreateAttendanceEntryResponse> CreateAttendanceEntryAsync(AttendanceEntryDto attendanceEntryDto);
        Task<UpdateAttendanceEntryResponse> UpdateAttendanceEntryAsync(AttendanceEntryDto attendanceEntryDto);
        Task<DeleteAttendanceEntryResponse> DeleteAttendanceEntryAsync(AttendanceEntryDto attendanceEntryDto);
        Task<GetEmployeeAttendanceEntriesResponse> GetEmployeeAttendanceEntriesAsync(int employeeId);
    }
}