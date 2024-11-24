using System.Threading.Tasks;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;

namespace Charmaran.Application.Contracts.AttendanceTracker
{
    public interface IEmployeeService
    {
        Task<CreateEmployeeResponse> CreateEmployeeAsync(string name);
        Task<DeleteEmployeeResponse> DeleteEmployeeAsync(int id);
        Task<UpdateEmployeeResponse> UpdateEmployeeAsync(int id, string name);
        Task<PermanentDeleteEmployeeResponse> PermanentDeleteEmployeeAsync(int id);
        Task<bool> RestoreEmployeeAsync(int id);
        Task<GetEmployeeResponse> GetEmployeeByIdAsync(int id);
        Task<GetAllEmployeesResponse> GetEmployeesAsync();
        Task ExportEmployeesAsync();
    }
}