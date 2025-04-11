using System.Collections.Generic;
using Charmaran.Shared.AttendanceTracker;

namespace Charmaran.UI.Models
{
    public class EmployeeDetailed
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public List<AttendanceEntryDto> AttendanceEntries { get; set; } = new List<AttendanceEntryDto>();
        public double LatePoints30 { get; set; }
        public double LatePoints180 { get; set; }
        public double LeftEarlyPoints30 { get; set; }
        public double LeftEarlyPoints180 { get; set; }
        public double UnExcusedAbsencePoints30 { get; set; }
        public double UnExcusedAbsencePoints180 { get; set; }
        public double NoCallNoShowPoints30 { get; set; }
        public double NoCallNoShowPoints180 { get; set; }
        public double VacationUsed { get; set; }
        public double ExcusedAbsencePoints180 { get; set; }
        public double TotalPoints30Days { get; set; }
        public double TotalPoints180Days { get; set; }
    }
}