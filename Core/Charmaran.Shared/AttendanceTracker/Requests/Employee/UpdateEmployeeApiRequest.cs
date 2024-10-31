namespace Charmaran.Shared.AttendanceTracker.Requests.Employee
{
    /// <summary>
    /// Request to update an employee.
    /// </summary>
    public class UpdateEmployeeApiRequest
    {
        /// <summary>
        /// Id of the employee to update
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Name of the employee to update to
        /// </summary>
        public string? Name { get; set; }
    }
}