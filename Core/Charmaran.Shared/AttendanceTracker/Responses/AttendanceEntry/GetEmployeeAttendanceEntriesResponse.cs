using System.Collections.Generic;

namespace Charmaran.Shared.AttendanceTracker.Responses.AttendanceEntry
{
    public class GetEmployeeAttendanceEntriesResponse : BaseResponse
    {
        public IEnumerable<AttendanceEntryDto> AttendanceEntries { get; set; }
    }
}