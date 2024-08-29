using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Charmaran.Application.Contracts.AttendanceTracker;
using Charmaran.Application.Validators.AttendanceTracker;
using Charmaran.Domain.Entities.AttendanceTracker;
using Charmaran.Persistence.Contracts.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Charmaran.Application.Services.AttendanceTracker
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ILogger<EmployeeService> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            this._logger = logger;
            this._employeeRepository = employeeRepository;
            this._mapper = mapper;
        }
        
        public async Task<CreateEmployeeResponse> CreateEmployeeAsync(string name)
        {
            //Validate the create request
            CreateEmployeeValidator validator = new CreateEmployeeValidator(this._employeeRepository);
            ValidationResult? validateResult = await validator.ValidateAsync(new Employee { Name = name });
            
            CreateEmployeeResponse createEmployeeResponse = new CreateEmployeeResponse
            {
                Success = true,
                Message = "Employee Creation Successful"
            };

            //Request is invalid
            if (validateResult.Errors.Count > 0)
            {
                createEmployeeResponse.Message = "Employee Creation Invalid";
                createEmployeeResponse.Success = false;
                createEmployeeResponse.ValidationErrors = new List<string>();
                
                foreach (ValidationFailure? validationError in validateResult.Errors)
                {
                    createEmployeeResponse.ValidationErrors.Add(validationError.ErrorMessage);
                }
            }
            //Valid Request
            else
            {
                int newId = await this._employeeRepository.AddAsync(new Employee { Name = name });
                
                //Create failed send back failed message
                if (newId == -1)
                {
                    createEmployeeResponse.Success = false;
                    createEmployeeResponse.Message = "Employee Creation Failed";
                    return createEmployeeResponse;
                }
                
                //Create succeeded send back details
                createEmployeeResponse.EmployeeDto = new EmployeeDetailDto
                {
                    Id = newId,
                    Name = name
                };
            }
            
            return createEmployeeResponse;
        }

        public async Task<DeleteEmployeeResponse> DeleteEmployeeAsync(int id)
        {
            DeleteEmployeeResponse deleteEmployeeResponse = new DeleteEmployeeResponse
            {
                Success = true,
                Message = "Employee Deleted Successfully"
            };
            
            Employee? employee = await this._employeeRepository.GetByIdAsync(id);
            
            if (employee == null)
            {
                deleteEmployeeResponse.Success = false;
                deleteEmployeeResponse.Message = "Employee Not Found";
                return deleteEmployeeResponse;
            }
            
            employee.IsDeleted = true;
            employee.LastModifiedBy = "system";
            employee.LastModifiedDate = DateTime.UtcNow;

            bool result = await this._employeeRepository.UpdateAsync(employee);
            
            if (result == false)
            {
                deleteEmployeeResponse.Success = false;
                deleteEmployeeResponse.Message = "Failed to Delete Employee";
            }
            
            return deleteEmployeeResponse;
        }
        
        public async Task<UpdateEmployeeResponse> UpdateEmployeeAsync(int id, string name)
        {
            UpdateEmployeeResponse updateEmployeeResponse = new UpdateEmployeeResponse();
            
            Employee? existingEmployee = await this._employeeRepository.GetByIdAsync(id);
            
            if (existingEmployee == null)
            {
                updateEmployeeResponse.Success = false;
                updateEmployeeResponse.Message = "Employee Not Found";
                return updateEmployeeResponse;
            }
            
            UpdateEmployeeValidator updateEmployeeValidator = new UpdateEmployeeValidator(this._employeeRepository);
            ValidationResult validationResult = await updateEmployeeValidator.ValidateAsync(new Employee { Id = id, Name = name });
            
            if (validationResult.Errors.Count > 0)
            {
                updateEmployeeResponse.Success = false;
                updateEmployeeResponse.Message = "Employee Update Invalid";
                updateEmployeeResponse.ValidationErrors = new List<string>();
                
                foreach (ValidationFailure? error in validationResult.Errors)
                {
                    updateEmployeeResponse.ValidationErrors.Add(error.ErrorMessage);
                }
                
                return updateEmployeeResponse;
            }
            
            existingEmployee.Name = name;
            existingEmployee.LastModifiedBy = "system";
            existingEmployee.LastModifiedDate = DateTime.UtcNow;
            
            bool result = await this._employeeRepository.UpdateAsync(existingEmployee);
            
            if (result == false)
            {
                updateEmployeeResponse.Success = false;
                updateEmployeeResponse.Message = "Failed to Update Employee";
            }
            
            return updateEmployeeResponse;
        }
        
        public async Task<bool> PermanentDeleteEmployeeAsync(int id)
        {
            Employee? employee = await this._employeeRepository.GetByIdAsync(id);
            
            if (employee == null)
            {
                return false;
            }
            
            return await this._employeeRepository.DeleteAsync(employee);
        }
        
        public async Task<bool> RestoreEmployeeAsync(int id)
        {
            Employee? employee = await this._employeeRepository.GetByIdAsync(id);
            
            if (employee == null)
            {
                return false;
            }
            
            employee.IsDeleted = false;
            employee.LastModifiedBy = "system";
            employee.LastModifiedDate = DateTime.UtcNow;
            
            return await this._employeeRepository.UpdateAsync(employee);
        }
        
        public async Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int id)
        {
            Employee? employee = await this._employeeRepository.GetByIdAsync(id);
            
            if (employee == null)
            {
                return null;
            }

            EmployeeDetailDto? employeeDto = this._mapper.Map<EmployeeDetailDto>(employee);
            
            return employeeDto;
        }
        
        public async Task<List<EmployeeDto>> GetEmployeesAsync()
        {
            IEnumerable<Employee>? allEmployees = await this._employeeRepository.ListAllAsync();
            
            if (allEmployees == null)
            {
                return new List<EmployeeDto>();
            }
            
            allEmployees = allEmployees.OrderBy(e => e.Name).ToList();
            return this._mapper.Map<List<EmployeeDto>>(allEmployees);
        }

        public Task ExportEmployeesAsync()
        {
            throw new NotImplementedException();
        }
    }
}