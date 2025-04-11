using System.Collections.Generic;
using System.Threading.Tasks;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.AttendanceEntry;
using Charmaran.UI.Contracts;
using Charmaran.UI.Models;

namespace Charmaran.UI.Services
{
    public class AttendanceEntryService : IAttendanceEntryService
    {
        public Task<IEnumerable<AttendanceEntryDto>> GetAttendanceEntries(int employeeId, int year)
        {
            throw new System.NotImplementedException();
        }

        public Task<CreateAttendanceEntryResponse> AddAttendanceEntry(AttendanceEntryModel entry)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteAttendanceEntry(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<UpdateAttendanceEntryResponse> UpdateAttendanceEntry(AttendanceEntryDto entry)
        {
            throw new System.NotImplementedException();
        }
    }
}