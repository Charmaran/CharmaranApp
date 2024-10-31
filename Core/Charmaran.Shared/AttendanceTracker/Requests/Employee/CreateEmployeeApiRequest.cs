namespace Charmaran.Shared.AttendanceTracker.Requests.Employee
{
    /// <summary>
    /// Request to create a new employee.
    /// </summary>
    public class CreateEmployeeApiRequest
    {
        /// <summary>
        /// The name of the employee.
        /// </summary>
        public string? Name { get; set; }
    }
}