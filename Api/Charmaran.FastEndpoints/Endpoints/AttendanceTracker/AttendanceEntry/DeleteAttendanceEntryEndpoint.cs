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
    public class DeleteAttendanceEntryEndpoint : Endpoint<DeleteAttendanceEntryApiRequest, DeleteAttendanceEntryResponse>
    {
        private readonly ILogger<DeleteAttendanceEntryEndpoint> _logger;
        private readonly IAttendanceEntryService _attendanceEntryService;

        public DeleteAttendanceEntryEndpoint(ILogger<DeleteAttendanceEntryEndpoint> logger, IAttendanceEntryService attendanceEntryService)
        {
            this._logger = logger;
            this._attendanceEntryService = attendanceEntryService;
        }
        
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Delete("api/attendanceentry");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("AttendanceEntry"));
            this.Version(1);
        }
        
        public override async Task HandleAsync(DeleteAttendanceEntryApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Delete Attendance Entry Request");

            DeleteAttendanceEntryResponse deleteAttendanceEntryResponse;
            try
            {
                deleteAttendanceEntryResponse = await this._attendanceEntryService.DeleteAttendanceEntryAsync(req.AttendanceEntry);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error deleting attendance entry");
                deleteAttendanceEntryResponse = new DeleteAttendanceEntryResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
            }
            
            await this.SendAsync(deleteAttendanceEntryResponse, cancellation: ct);
        }
    }
}