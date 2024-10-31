namespace Charmaran.Shared.AttendanceTracker.Requests.Employee
{
    /// <summary>
    /// Request to delete an employee.
    /// </summary>
    public class DeleteEmployeeApiRequest
    {
        /// <summary>
        /// The id of the employee to delete.
        /// </summary>
        public int Id { get; set; }
    }
}