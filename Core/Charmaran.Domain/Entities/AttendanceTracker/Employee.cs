using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Charmaran.Domain.Common;

namespace Charmaran.Domain.Entities.AttendanceTracker
{
    public class Employee : AuditableEntity
    {
        public int Id { get; set; }
        
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        
        public bool IsDeleted { get; set; }
        
        public IEnumerable<AttendanceEntry>? AttendanceEntries { get; set; }
    }
}