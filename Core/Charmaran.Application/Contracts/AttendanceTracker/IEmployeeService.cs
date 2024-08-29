using System.Collections.Generic;
using System.Threading.Tasks;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;

namespace Charmaran.Application.Contracts.AttendanceTracker
{
    public interface IEmployeeService
    {
        Task<CreateEmployeeResponse> CreateEmployeeAsync(string name);
        Task<DeleteEmployeeResponse> DeleteEmployeeAsync(int id);
        Task<UpdateEmployeeResponse> UpdateEmployeeAsync(int id, string name);
        Task<bool> PermanentDeleteEmployeeAsync(int id);
        Task<bool> RestoreEmployeeAsync(int id);
        Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int id);
        Task<List<EmployeeDto>> GetEmployeesAsync();
        Task ExportEmployeesAsync();
    }
}