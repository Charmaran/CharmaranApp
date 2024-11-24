namespace Charmaran.Shared.AttendanceTracker.Requests.Employee
{
    /// <summary>
    /// Request to get an employee.
    /// </summary>
    public class GetEmployeeApiRequest
    {
        /// <summary>
        /// The id of the employee to get.
        /// </summary>
        public int Id { get; set; }
    }
}