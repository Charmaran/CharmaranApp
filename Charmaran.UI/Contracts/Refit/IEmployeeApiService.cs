using System.Collections.Generic;
using System.Threading.Tasks;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using Refit;

namespace Charmaran.UI.Contracts.Refit
{
    public interface IEmployeeApiService
    {
        [Get("/api/employees/all")]
        public Task<ApiResponse<GetAllEmployeesResponse>> GetEmployees();

        [Get("/api/employee?id={employeeId}")]
        public Task<ApiResponse<GetEmployeeResponse>> GetEmployee(int employeeId);

        [Post("/api/employee/create")]
        public Task<ApiResponse<CreateEmployeeResponse>> AddEmployee([Body] string name);

        [Delete("/api/employee")]
        public Task<ApiResponse<DeleteEmployeeResponse>> DeleteEmployee(int employeeId);

        [Delete("/api/employee/delete")]
        public Task<ApiResponse<PermanentDeleteEmployeeResponse>> PermDeleteEmployee(int employeeId);

        [Put("/api/employee/update")]
        public Task<ApiResponse<UpdateEmployeeResponse>> UpdateEmployee([Body] EmployeeDetailDto employee);
    }
}