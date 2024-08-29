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
            //Log the request
            this._logger.LogInformation($"Creating Employee with name: {name}");
            
            //Create the response
            CreateEmployeeResponse createEmployeeResponse = new CreateEmployeeResponse
            {
                Success = true,
                Message = "Employee Creation Successful"
            };
            
            //Validate the create request
            CreateEmployeeValidator validator = new CreateEmployeeValidator(this._employeeRepository);
            ValidationResult? validateResult = await validator.ValidateAsync(new Employee { Name = name });

            //Request is invalid
            if (validateResult.Errors.Count > 0)
            {
                this._logger.LogWarning("Employee failed validation for creation, returning validation errors");
                
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
                this._logger.LogInformation("Employee passed validation for creation, creating employee");
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
            
            //Log and return the response
            this._logger.LogInformation("Returning response for employee creation");
            return createEmployeeResponse;
        }

        public async Task<DeleteEmployeeResponse> DeleteEmployeeAsync(int id)
        {
            //Log the request
            this._logger.LogInformation($"Deleting Employee with id: {id}");
            
            //Create the response
            DeleteEmployeeResponse deleteEmployeeResponse = new DeleteEmployeeResponse
            {
                Success = true,
                Message = "Employee Deleted Successfully"
            };
            
            //Get the employee
            Employee? employee = await this._employeeRepository.GetByIdAsync(id);
            
            //Employee not found
            if (employee == null)
            {
                this._logger.LogWarning($"Employee with id: {id} not found, returning failed response");
                
                deleteEmployeeResponse.Success = false;
                deleteEmployeeResponse.Message = "Employee Not Found";
                return deleteEmployeeResponse;
            }
            
            //Delete the employee
            employee.IsDeleted = true;
            employee.LastModifiedBy = "system";
            employee.LastModifiedDate = DateTime.UtcNow;

            this._logger.LogInformation("Deleting employee");
            bool result = await this._employeeRepository.UpdateAsync(employee);
            
            if (result == false)
            {
                this._logger.LogWarning("Failed to delete employee, returning failed response");
                
                deleteEmployeeResponse.Success = false;
                deleteEmployeeResponse.Message = "Failed to Delete Employee";
            }
            
            //Log and return the response
            this._logger.LogInformation("Returning response for employee deletion");
            return deleteEmployeeResponse;
        }
        
        public async Task<UpdateEmployeeResponse> UpdateEmployeeAsync(int id, string name)
        {
            //Log the request
            this._logger.LogInformation($"Updating Employee with id: {id}");
            
            //Create the response
            UpdateEmployeeResponse updateEmployeeResponse = new UpdateEmployeeResponse
            {
                Success = true,
                Message = "Employee Update Successful"
            };
            
            //Get the employee
            Employee? existingEmployee = await this._employeeRepository.GetByIdAsync(id);
            
            //Employee not found
            if (existingEmployee == null)
            {
                this._logger.LogWarning($"Employee with id: {id} not found, returning failed response");
                
                updateEmployeeResponse.Success = false;
                updateEmployeeResponse.Message = "Employee Not Found";
                return updateEmployeeResponse;
            }
            
            //Validate the update request
            UpdateEmployeeValidator updateEmployeeValidator = new UpdateEmployeeValidator(this._employeeRepository);
            ValidationResult validationResult = await updateEmployeeValidator.ValidateAsync(new Employee { Id = id, Name = name });
            
            //Request is invalid
            if (validationResult.Errors.Count > 0)
            {
                this._logger.LogWarning("Employee failed validation for update, returning validation errors");
                
                updateEmployeeResponse.Success = false;
                updateEmployeeResponse.Message = "Employee Update Invalid";
                updateEmployeeResponse.ValidationErrors = new List<string>();
                
                foreach (ValidationFailure? error in validationResult.Errors)
                {
                    updateEmployeeResponse.ValidationErrors.Add(error.ErrorMessage);
                }
                
                return updateEmployeeResponse;
            }
            
            //Modify the employee
            existingEmployee.Name = name;
            existingEmployee.LastModifiedBy = "system";
            existingEmployee.LastModifiedDate = DateTime.UtcNow;
            
            this._logger.LogInformation("Updating employee");
            bool result = await this._employeeRepository.UpdateAsync(existingEmployee);
            
            //Update failed
            if (result == false)
            {
                this._logger.LogWarning("Failed to update employee, returning failed response");
                
                updateEmployeeResponse.Success = false;
                updateEmployeeResponse.Message = "Failed to Update Employee";
            }
            
            //Log and return the response
            this._logger.LogInformation("Returning response for employee update");
            return updateEmployeeResponse;
        }
        
        public async Task<bool> PermanentDeleteEmployeeAsync(int id)
        {
            //Log the request
            this._logger.LogInformation($"Permanently Deleting Employee with id: {id}");
            
            //Get the employee
            Employee? employee = await this._employeeRepository.GetByIdAsync(id);
            
            //Employee not found
            if (employee == null)
            {
                this._logger.LogWarning($"Employee with id: {id} not found, returning failed response");
                return false;
            }
            
            //Delete the employee
            this._logger.LogInformation("Permanently deleting employee");
            bool success = await this._employeeRepository.DeleteAsync(employee);
            
            //Delete failed
            if (success == false)
            {
                this._logger.LogWarning("Failed to permanently delete employee, returning failed response");
            }
            
            //Log and return the response
            this._logger.LogInformation("Returning response for employee permanent deletion");
            return success;
        }
        
        public async Task<bool> RestoreEmployeeAsync(int id)
        {
            //Log the request
            this._logger.LogInformation($"Restoring Employee with id: {id}");
            
            //Get the employee
            Employee? employee = await this._employeeRepository.GetByIdAsync(id);
            
            //Employee not found
            if (employee == null)
            {
                this._logger.LogWarning($"Employee with id: {id} not found, returning failed response");
                return false;
            }
            
            //Restore the employee
            employee.IsDeleted = false;
            employee.LastModifiedBy = "system";
            employee.LastModifiedDate = DateTime.UtcNow;
            
            //Update the employee
            this._logger.LogInformation("Restoring employee");
            bool success = await this._employeeRepository.UpdateAsync(employee);
            
            //Update failed
            if (success == false)
            {
                this._logger.LogWarning("Failed to restore employee, returning failed response");
            }
            
            //Log and return the response
            this._logger.LogInformation("Returning response for employee restoration");
            return success;
        }
        
        public async Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int id)
        {
            //Log the request
            this._logger.LogInformation($"Getting Employee with id: {id}");
            
            //Get the employee
            Employee? employee = await this._employeeRepository.GetByIdAsync(id);
            
            //Employee not found
            if (employee == null)
            {
                this._logger.LogWarning($"Employee with id: {id} not found, returning null");
                return null;
            }

            //Log and return the response
            this._logger.LogInformation("Returning response for employee retrieval");
            EmployeeDetailDto? employeeDto = this._mapper.Map<EmployeeDetailDto>(employee);
            
            return employeeDto;
        }
        
        public async Task<List<EmployeeDto>> GetEmployeesAsync()
        {
            //Log the request
            this._logger.LogInformation("Getting all Employees");
            
            //Get all employees
            IEnumerable<Employee>? allEmployees = await this._employeeRepository.ListAllAsync();
            
            //No employees found
            if (allEmployees == null)
            {
                this._logger.LogWarning("No employees found, returning empty list");
                return new List<EmployeeDto>();
            }
            
            //Log and return the response
            this._logger.LogInformation("Returning response for all employees retrieval");
            allEmployees = allEmployees.OrderBy(e => e.Name).ToList();
            return this._mapper.Map<List<EmployeeDto>>(allEmployees);
        }

        public Task ExportEmployeesAsync()
        {
            throw new NotImplementedException();
        }
    }
}