using System.Threading.Tasks;
using Charmaran.Shared.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Requests.AttendanceEntry;
using Charmaran.Shared.AttendanceTracker.Responses.AttendanceEntry;
using Charmaran.UI.Contracts;
using Charmaran.UI.Contracts.Refit;
using Charmaran.UI.Models;
using Newtonsoft.Json;
using Refit;

namespace Charmaran.UI.Services
{
    public class AttendanceEntryService : IAttendanceEntryService
    {
        private readonly IAttendanceEntryApiService _attendanceEntryApiService;

        public AttendanceEntryService(IAttendanceEntryApiService attendanceEntryApiService)
        {
            this._attendanceEntryApiService = attendanceEntryApiService;
        }
        public async Task<GetEmployeeAttendanceEntriesResponse> GetAttendanceEntries(int employeeId, int year)
        {
            //TODO: Add year to filter results in the backend
            ApiResponse<GetEmployeeAttendanceEntriesResponse> response = await this._attendanceEntryApiService.GetAttendanceEntries(new GetEmployeeAttendanceEntriesApiRequest
            {
                EmployeeId = employeeId,
            });

            if (response.IsSuccessStatusCode)
            {
                return response.Content!;
            }
            
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new GetEmployeeAttendanceEntriesResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<GetEmployeeAttendanceEntriesResponse>(response.Error.Content)!;
        }

        public async Task<CreateAttendanceEntryResponse> AddAttendanceEntry(AttendanceEntryModel entry)
        {
            AttendanceEntryDto entryDto = new AttendanceEntryDto
            {
                EmployeeId = entry.EmployeeId,
                Category = entry.Category,
                Amount = entry.Amount,
                InputDate = entry.InputDate,
                Notes = entry.Notes
            };
            
            ApiResponse<CreateAttendanceEntryResponse> response = await this._attendanceEntryApiService.AddAttendanceEntry(new CreateAttendanceEntryApiRequest
            {
                AttendanceEntry = entryDto
            });

            if (response.IsSuccessStatusCode)
            {
                return response.Content!;
            }
            
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new CreateAttendanceEntryResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<CreateAttendanceEntryResponse>(response.Error.Content)!;
        }

        public async Task<DeleteAttendanceEntryResponse> DeleteAttendanceEntry(int id)
        {
            ApiResponse<DeleteAttendanceEntryResponse> response = await this._attendanceEntryApiService.DeleteAttendanceEntry(new DeleteAttendanceEntryApiRequest
            {
                Id = id
            });

            if (response.IsSuccessStatusCode)
            {
                return response.Content!;
            }
            
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new DeleteAttendanceEntryResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<DeleteAttendanceEntryResponse>(response.Error.Content)!;
        }

        public async Task<UpdateAttendanceEntryResponse> UpdateAttendanceEntry(AttendanceEntryDto entry)
        {
            ApiResponse<UpdateAttendanceEntryResponse> response = await this._attendanceEntryApiService.UpdateAttendanceEntry(new UpdateAttendanceEntryApiRequest
            {
                AttendanceEntry = entry
            });

            if (response.IsSuccessStatusCode)
            {
                return response.Content!;
            }
            
            return string.IsNullOrEmpty(response.Error.Content) ? 
                new UpdateAttendanceEntryResponse { Success = false, Message = "Unexpected Error Occurred" } 
                : JsonConvert.DeserializeObject<UpdateAttendanceEntryResponse>(response.Error.Content)!;
        }
    }
}