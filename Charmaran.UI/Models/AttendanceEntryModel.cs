using System;
using Charmaran.Shared.AttendanceTracker.Enums;

namespace Charmaran.UI.Models
{
    public class AttendanceEntryModel
    {
        public int EmployeeId { get; set; }
        public AttendanceEntryCategory Category { get; set; }
        public float Amount { get; set; }
        public DateTime InputDate { get; set; }
        public string? Notes { get; set; }
        public bool IsNewEntry { get; set; }
    }
}