using System;
using Charmaran.Domain.Common;

namespace Charmaran.Domain.Entities.AttendanceTracker
{
    public class Holiday : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}