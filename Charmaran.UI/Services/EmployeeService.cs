using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using Charmaran.UI.Contracts;
using Charmaran.UI.Contracts.Refit;
using Charmaran.UI.Models;
using Newtonsoft.Json;
using Refit;

namespace Charmaran.UI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeApiService _employeeApiService;

        public EmployeeService(IEmployeeApiService employeeApiService)
        {
            this._employeeApiService = employeeApiService;
        }
        public async Task<GetAllEmployeesResponse> GetEmployees(bool includeDeleted)
        {
            ApiResponse<GetAllEmployeesResponse> response = await this._employeeApiService.GetEmployees();

            if (response.IsSuccessStatusCode)
            {
                return response.Content!;
            }
            
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new GetAllEmployeesResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<GetAllEmployeesResponse>(response.Error.Content)!;
        }

        public async Task<GetEmployeeResponse> GetEmployee(int employeeId)
        {
            ApiResponse<GetEmployeeResponse> response = await this._employeeApiService.GetEmployee(employeeId);

            if (response.IsSuccessStatusCode)
            {
                return response.Content!;
            }
			
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new GetEmployeeResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<GetEmployeeResponse>(response.Error.Content)!;
        }

        public async Task<CreateEmployeeResponse> AddEmployee(string name)
        {
            ApiResponse<CreateEmployeeResponse> response = await this._employeeApiService.AddEmployee(name);
            
            if (response.IsSuccessStatusCode)
            {
                return response.Content!;
            }
			
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new CreateEmployeeResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<CreateEmployeeResponse>(response.Error.Content)!;
        }

        public async Task<DeleteEmployeeResponse> DeleteEmployee(int employeeId)
        {
            ApiResponse<DeleteEmployeeResponse> response = await this._employeeApiService.DeleteEmployee(employeeId);
            
            if (response.IsSuccessStatusCode)
            {
                return response.Content!;
            }
			
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new DeleteEmployeeResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<DeleteEmployeeResponse>(response.Error.Content)!;
        }

        public async Task<PermanentDeleteEmployeeResponse> PermDeleteEmployee(int employeeId)
        {
            ApiResponse<PermanentDeleteEmployeeResponse> response = await this._employeeApiService.PermDeleteEmployee(employeeId);
            
            if (response.IsSuccessStatusCode)
            {
                return response.Content!;
            }
			
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new PermanentDeleteEmployeeResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<PermanentDeleteEmployeeResponse>(response.Error.Content)!;
        }

        public async Task<UpdateEmployeeResponse> UpdateEmployee(EmployeeDetailed employee)
        {
            EmployeeDetailDto employeeDto = new EmployeeDetailDto
            {
                Id = employee.Id,
                Name = employee.Name,
                IsDeleted = employee.IsDeleted,
                AttendanceEntries = employee.AttendanceEntries
            };
            
            ApiResponse<UpdateEmployeeResponse> response = await this._employeeApiService.UpdateEmployee(employeeDto);
            
            if (response.IsSuccessStatusCode)
            {
                return response.Content!;
            }
			
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new UpdateEmployeeResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<UpdateEmployeeResponse>(response.Error.Content)!;
        }
    }
}