using System.Collections.Generic;
using Charmaran.Domain.Common;

namespace Charmaran.Domain.Entities.AttendanceTracker
{
    public class Employee : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public IEnumerable<AttendanceEntry>? AttendanceEntries { get; set; }
    }
}