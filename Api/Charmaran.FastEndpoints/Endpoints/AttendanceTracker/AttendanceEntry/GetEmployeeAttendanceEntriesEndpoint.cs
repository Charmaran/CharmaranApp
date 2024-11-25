using System;
using System.Threading;
using System.Threading.Tasks;
using Charmaran.Application.Contracts.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Requests.AttendanceEntry;
using Charmaran.Shared.AttendanceTracker.Responses.AttendanceEntry;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Charmaran.FastEndpoints.Endpoints.AttendanceTracker.AttendanceEntry
{
    public class GetEmployeeAttendanceEntriesEndpoint : Endpoint<GetEmployeeAttendanceEntriesApiRequest, GetEmployeeAttendanceEntriesResponse>
    {
        private readonly ILogger<GetEmployeeAttendanceEntriesEndpoint> _logger;
        private readonly IAttendanceEntryService _attendanceEntryService;

        public GetEmployeeAttendanceEntriesEndpoint(ILogger<GetEmployeeAttendanceEntriesEndpoint> logger, IAttendanceEntryService attendanceEntryService)
        {
            this._logger = logger;
            this._attendanceEntryService = attendanceEntryService;
        }
        
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Get("api/attendanceentry");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("AttendanceEntry"));
            this.Version(1);
        }
        
        public override async Task HandleAsync(GetEmployeeAttendanceEntriesApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Get Employee Attendance Entries Request");

            GetEmployeeAttendanceEntriesResponse getEmployeeAttendanceEntriesResponse;
            try
            {
                getEmployeeAttendanceEntriesResponse = await this._attendanceEntryService.GetEmployeeAttendanceEntriesAsync(req.EmployeeId);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error getting employee attendance entries");
                getEmployeeAttendanceEntriesResponse = new GetEmployeeAttendanceEntriesResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
            }
            
            await this.SendAsync(getEmployeeAttendanceEntriesResponse, cancellation: ct);
        }
    }
}