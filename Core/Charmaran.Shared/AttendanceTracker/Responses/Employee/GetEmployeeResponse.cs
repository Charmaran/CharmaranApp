namespace Charmaran.Shared.AttendanceTracker.Responses.Employee
{
    /// <summary>
    /// Response to get an employee.
    /// </summary>
    public class GetEmployeeResponse : BaseResponse
    {
        /// <summary>
        /// The employee requested if found.
        /// </summary>
        public EmployeeDetailDto? Employee { get; set; }
    }
}