using System.Collections.Generic;
using System.Threading.Tasks;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.AttendanceEntry;
using Charmaran.UI.Models;

namespace Charmaran.UI.Contracts
{
    public interface IAttendanceEntryService
    {
        Task<IEnumerable<AttendanceEntryDto>> GetAttendanceEntries(int employeeId, int year);
        Task<CreateAttendanceEntryResponse> AddAttendanceEntry(AttendanceEntryModel entry);
        Task<bool> DeleteAttendanceEntry(int id);
        Task<UpdateAttendanceEntryResponse> UpdateAttendanceEntry(AttendanceEntryDto entry);
    }
}