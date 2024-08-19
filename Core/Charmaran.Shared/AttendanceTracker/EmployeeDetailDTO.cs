using System.Collections.Generic;

namespace Charmaran.Shared.AttendanceTracker
{
    public class EmployeeDetailDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsDeleted { get; set; }
        public List<AttendanceEntryDto> AttendanceEntries { get; set; } = new List<AttendanceEntryDto>();
    }
}