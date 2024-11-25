using System.Collections.Generic;

namespace Charmaran.Shared.AttendanceTracker.Responses.Employee
{
    /// <summary>
    /// Response to get all employees.
    /// </summary>
    public class GetAllEmployeesResponse : BaseResponse
    {
        /// <summary>
        /// The collection of employees.
        /// </summary>
        public IEnumerable<EmployeeDto>? Employees { get; set; }
    }
}