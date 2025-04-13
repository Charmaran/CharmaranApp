using System.Collections.Generic;
using System.Threading.Tasks;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using Charmaran.UI.Models;

namespace Charmaran.UI.Contracts
{
    public interface IEmployeeService
    {
        Task<GetAllEmployeesResponse> GetEmployees(bool includeDeleted);
        Task<GetEmployeeResponse> GetEmployee(int employeeId);
        Task<CreateEmployeeResponse> AddEmployee(string name);
        Task<DeleteEmployeeResponse> DeleteEmployee(int employeeId);
        Task<PermanentDeleteEmployeeResponse> PermDeleteEmployee(int employeeId);
        Task<UpdateEmployeeResponse> UpdateEmployee(EmployeeDetailed employee);
    }
}