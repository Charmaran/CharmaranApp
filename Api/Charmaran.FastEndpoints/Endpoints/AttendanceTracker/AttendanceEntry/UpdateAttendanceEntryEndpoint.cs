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
    public class UpdateAttendanceEntryEndpoint : Endpoint<UpdateAttendanceEntryApiRequest, UpdateAttendanceEntryResponse>
    {
        private readonly ILogger<UpdateAttendanceEntryEndpoint> _logger;
        private readonly IAttendanceEntryService _attendanceEntryService;

        public UpdateAttendanceEntryEndpoint(ILogger<UpdateAttendanceEntryEndpoint> logger, IAttendanceEntryService attendanceEntryService)
        {
            this._logger = logger;
            this._attendanceEntryService = attendanceEntryService;
        }
        
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Put("api/attendanceentry");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("AttendanceEntry"));
        }
        
        public override async Task HandleAsync(UpdateAttendanceEntryApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Update Attendance Entry Request");

            UpdateAttendanceEntryResponse updateAttendanceEntryResponse;
            try
            {
                updateAttendanceEntryResponse = await this._attendanceEntryService.UpdateAttendanceEntryAsync(req.AttendanceEntry);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error updating attendance entry");
                updateAttendanceEntryResponse = new UpdateAttendanceEntryResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
            }
            
            await this.SendAsync(updateAttendanceEntryResponse, cancellation: ct);
        }
    }
}