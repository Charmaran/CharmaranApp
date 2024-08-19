using System;
using Charmaran.Shared.AttendanceTracker.Enums;

namespace Charmaran.Shared.AttendanceTracker
{
    public class AttendanceEntryDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public AttendanceEntryCategory Category { get; set; }
        public float Amount { get; set; }
        public DateTime InputDate { get; set; }
        public string? Notes { get; set; }
    }
}