using System;
using Charmaran.Domain.Common;
using Charmaran.Shared.AttendanceTracker.Enums;

namespace Charmaran.Domain.Entities.AttendanceTracker
{
    public class AttendanceEntry : AuditableEntity
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public AttendanceEntryCategory Category { get; set; }
        public float Amount { get; set; }
        public DateTime InputDate { get; set; }
        public string? Notes { get; set; }
    }
}