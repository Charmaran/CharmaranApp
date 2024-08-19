namespace Charmaran.Shared.AttendanceTracker
{
    public class EmployeeAttendanceReport
    {
        public string Name { get; set; } = null!;
        public double VacationUsed { get; set; } = 0;
        public double LateAttendanceEntries30 { get; set; } = 0;
        public double LateAttendanceEntries180 { get; set; } = 0;
        public double LeftEarlyAttendanceEntries30 { get; set; } = 0;
        public double LeftEarlyAttendanceEntries180 { get; set; } = 0;
        public double UnExcusedAbsenceAttendanceEntries30 { get; set; } = 0;
        public double UnExcusedAbsenceAttendanceEntries180 { get; set; } = 0;
        public double NoCallNoShowAttendanceEntries30 { get; set; } = 0;
        public double NoCallNoShowAttendanceEntries180 { get; set; } = 0;
        public double ExcusedAbsenceAttendanceEntries30 { get; set; } = 0;
        public double ExcusedAbsenceAttendanceEntries180 { get; set; } = 0;
    }
}