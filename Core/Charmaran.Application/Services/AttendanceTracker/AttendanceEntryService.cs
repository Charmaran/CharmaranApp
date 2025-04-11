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
using Charmaran.Shared.AttendanceTracker.Responses.AttendanceEntry;
using Charmaran.Shared.Extensions;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Charmaran.Application.Services.AttendanceTracker
{
    public class AttendanceEntryService : IAttendanceEntryService
    {
        private readonly ILogger<AttendanceEntryService> _logger;
        private readonly IAttendanceEntryRepository _attendanceEntryRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;

        public AttendanceEntryService(ILogger<AttendanceEntryService> logger, IAttendanceEntryRepository attendanceEntryRepository,
            IMapper mapper, IEmployeeRepository employeeRepository)
        {
            this._logger = logger;
            this._attendanceEntryRepository = attendanceEntryRepository;
            this._mapper = mapper;
            this._employeeRepository = employeeRepository;
        }

        public async Task<CreateAttendanceEntryResponse> CreateAttendanceEntryAsync(AttendanceEntryDto attendanceEntryDto)
        {
            //Log the request
            this._logger.LogInformation($"Creating attendance entry for employee with id: {attendanceEntryDto.EmployeeId}");
            
            //Create the response
            CreateAttendanceEntryResponse createAttendanceEntryResponse = new CreateAttendanceEntryResponse
            {
                Success = true,
                Message = "Attendance Entry Created Successfully"
            };
            
            //Get the employee
            Employee? employee = await this._employeeRepository.GetByIdAsync(attendanceEntryDto.EmployeeId);
            
            //Employee not found
            if (employee == null)
            {
                this._logger.LogWarning($"Employee with id: {attendanceEntryDto.EmployeeId} not found, returning failed response");
                
                createAttendanceEntryResponse.Success = false;
                createAttendanceEntryResponse.Message = "Employee Not Found";
                return createAttendanceEntryResponse;
            }
            //Create the attendance entry
            AttendanceEntry attendanceEntry = this._mapper.Map<AttendanceEntry>(attendanceEntryDto);
            attendanceEntry.CreatedBy = "system";
            attendanceEntry.CreatedDate = DateTime.UtcNow;
            attendanceEntry.InputDate = attendanceEntry.InputDate.ConvertToUtc();
            
            //Validate the create request
            CreateAttendanceEntryValidator validator = new CreateAttendanceEntryValidator(this._employeeRepository);
            ValidationResult? validateResult = await validator.ValidateAsync(attendanceEntry);
            
            //Request is invalid
            if (validateResult.Errors.Count > 0)
            {
                this._logger.LogWarning($"Invalid request, returning failed response");
                
                createAttendanceEntryResponse.Success = false;
                createAttendanceEntryResponse.Message = "Invalid Request";
                
                foreach (ValidationFailure? validationError in validateResult.Errors)
                {
                    createAttendanceEntryResponse.ValidationErrors.Add(validationError.ErrorMessage);
                }
                
                return createAttendanceEntryResponse;
            } 
            
            //Create the attendance entry
            int id = await this._attendanceEntryRepository.AddAsync(attendanceEntry);
            
            //Attendance entry not created
            if (id == -1)
            {
                this._logger.LogWarning($"Failed to create attendance entry, returning failed response");
                
                createAttendanceEntryResponse.Success = false;
                createAttendanceEntryResponse.Message = "Failed to Create Attendance Entry";
                return createAttendanceEntryResponse;
            }
            
            //Update the response
            attendanceEntry.Id = id;
            createAttendanceEntryResponse.AttendanceEntryDto = this._mapper.Map<AttendanceEntryDto>(attendanceEntry);
            
            return createAttendanceEntryResponse;
        }
        
        public async Task<UpdateAttendanceEntryResponse> UpdateAttendanceEntryAsync(AttendanceEntryDto attendanceEntryDto)
        {
            //Log the request
            this._logger.LogInformation($"Updating attendance entry for employee with id: {attendanceEntryDto.EmployeeId}");
            
            //Create the response
            UpdateAttendanceEntryResponse updateAttendanceEntryResponse = new UpdateAttendanceEntryResponse
            {
                Success = true,
                Message = "Attendance Entry Updated Successfully"
            };
            
            //Get the employee
            Employee? employee = await this._employeeRepository.GetByIdAsync(attendanceEntryDto.EmployeeId);
            
            //Employee not found
            if (employee == null)
            {
                this._logger.LogWarning($"Employee with id: {attendanceEntryDto.EmployeeId} not found, returning failed response");
                
                updateAttendanceEntryResponse.Success = false;
                updateAttendanceEntryResponse.Message = "Employee Not Found";
                return updateAttendanceEntryResponse;
            }
            
            //Update the attendance entry
            AttendanceEntry attendanceEntry = this._mapper.Map<AttendanceEntry>(attendanceEntryDto);
            attendanceEntry.LastModifiedBy = "system";
            attendanceEntry.LastModifiedDate = DateTime.UtcNow;
            
            //Validate the update request
            UpdateAttendanceEntryValidator validator = new UpdateAttendanceEntryValidator(this._employeeRepository);
            ValidationResult? validateResult = await validator.ValidateAsync(attendanceEntry);
            
            //Request is invalid
            if (validateResult.Errors.Count > 0)
            {
                this._logger.LogWarning($"Invalid request, returning failed response");
                
                updateAttendanceEntryResponse.Success = false;
                updateAttendanceEntryResponse.Message = "Invalid Request";
                
                foreach (ValidationFailure? validationError in validateResult.Errors)
                {
                    updateAttendanceEntryResponse.ValidationErrors.Add(validationError.ErrorMessage);
                }
                
                return updateAttendanceEntryResponse;
            }
            
            //Update the attendance entry
            bool success = await this._attendanceEntryRepository.UpdateAsync(attendanceEntry);
            
            //Attendance entry not updated
            if (success == false)
            {
                this._logger.LogWarning($"Failed to update attendance entry, returning failed response");
                
                updateAttendanceEntryResponse.Success = false;
                updateAttendanceEntryResponse.Message = "Failed to Update Attendance Entry";
            }
            
            //Return the response
            return updateAttendanceEntryResponse;
        }
        
        public async Task<DeleteAttendanceEntryResponse> DeleteAttendanceEntryAsync(int id)
        {
            //Log the request
            this._logger.LogInformation($"Deleting attendance entry with id: {id}");
            
            //Create the response
            DeleteAttendanceEntryResponse deleteAttendanceEntryResponse = new DeleteAttendanceEntryResponse
            {
                Success = true,
                Message = "Attendance Entry Deleted Successfully"
            };
            
            //Get the attendance entry
            AttendanceEntry? attendanceEntry = await this._attendanceEntryRepository.GetByIdAsync(id);
            
            //Attendance entry not found
            if (attendanceEntry == null)
            {
                this._logger.LogWarning($"Attendance entry with id: {id} not found, returning failed response");
                
                deleteAttendanceEntryResponse.Success = false;
                deleteAttendanceEntryResponse.Message = "Attendance Entry Not Found";
                return deleteAttendanceEntryResponse;
            }
            
            //Delete the attendance entry
            bool success = await this._attendanceEntryRepository.DeleteAsync(attendanceEntry);
            
            //Attendance entry not deleted
            if (success == false)
            {
                this._logger.LogWarning($"Failed to delete attendance entry, returning failed response");
                
                deleteAttendanceEntryResponse.Success = false;
                deleteAttendanceEntryResponse.Message = "Failed to Delete Attendance Entry";
            }
            
            //Return the response
            return deleteAttendanceEntryResponse;
        }
        
        public async Task<GetEmployeeAttendanceEntriesResponse> GetEmployeeAttendanceEntriesAsync(int employeeId)
        {
            //Log the request
            this._logger.LogInformation($"Getting attendance entries for employee with id: {employeeId}");
            
            //Create the response
            GetEmployeeAttendanceEntriesResponse getEmployeeAttendanceEntriesResponse = new GetEmployeeAttendanceEntriesResponse
            {
                Success = true,
                Message = "Successfully Fetched Employee Attendance Entries"
            };
            
            //Get the employee
            Employee? employee = await this._employeeRepository.GetByIdAsync(employeeId);
            
            //Employee not found
            if (employee == null)
            {
                this._logger.LogWarning($"Employee with id: {employeeId} not found, returning failed response");
                
                return new GetEmployeeAttendanceEntriesResponse
                {
                    Success = false,
                    Message = "Employee Not Found"
                };
            }
            
            //Get the attendance entries
            AttendanceEntry[]? attendanceEntries = (await this._attendanceEntryRepository.ListAsync(employeeId))?.ToArray();
            if (attendanceEntries == null)
            {
                this._logger.LogWarning($"No attendance entries found for employee with id: {employeeId}");
                
                return new GetEmployeeAttendanceEntriesResponse
                {
                    Success = true,
                    Message = "No Attendance Entries Found"
                };
            }

            // Convert to local time and map to dto
            List<AttendanceEntryDto> attendanceEntriesList = new List<AttendanceEntryDto>();
            foreach (AttendanceEntry attendanceEntry in attendanceEntries)
            {
                attendanceEntry.InputDate = attendanceEntry.InputDate.ConvertToLocalTime();
                attendanceEntriesList.Add(this._mapper.Map<AttendanceEntryDto>(attendanceEntry));
            }
            getEmployeeAttendanceEntriesResponse.AttendanceEntries = attendanceEntriesList;
            
            //Return the response
            return getEmployeeAttendanceEntriesResponse;
        }
    }
}