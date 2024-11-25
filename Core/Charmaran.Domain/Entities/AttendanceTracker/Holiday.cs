using System;
using System.ComponentModel.DataAnnotations;
using Charmaran.Domain.Common;

namespace Charmaran.Domain.Entities.AttendanceTracker
{
    public class Holiday : AuditableEntity
    {
        public int Id { get; set; }
        
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}