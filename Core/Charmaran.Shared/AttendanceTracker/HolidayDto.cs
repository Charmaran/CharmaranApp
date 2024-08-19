using System;

namespace Charmaran.Shared.AttendanceTracker
{
    public class HolidayDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime Date { get; set; }
    }
}