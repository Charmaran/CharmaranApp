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
    public class CreateAttendanceEntryEndpoint : Endpoint<CreateAttendanceEntryApiRequest, CreateAttendanceEntryResponse>
    {
        private readonly ILogger<CreateAttendanceEntryEndpoint> _logger;
        private readonly IAttendanceEntryService _attendanceEntryService;

        public CreateAttendanceEntryEndpoint(ILogger<CreateAttendanceEntryEndpoint> logger, IAttendanceEntryService attendanceEntryService)
        {
            this._logger = logger;
            this._attendanceEntryService = attendanceEntryService;
        }

        public override void Configure()
        {
            this.AllowAnonymous();
            this.Post("api/attendanceentry");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("AttendanceEntry"));
        }

        public override async Task HandleAsync(CreateAttendanceEntryApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Create Attendance Entry Request");

            CreateAttendanceEntryResponse createAttendanceEntryResponse;
            try
            {
                createAttendanceEntryResponse = await this._attendanceEntryService.CreateAttendanceEntryAsync(req.AttendanceEntry);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error creating attendance entry");
                createAttendanceEntryResponse = new CreateAttendanceEntryResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
            }
            
            await this.SendAsync(createAttendanceEntryResponse, cancellation: ct);
        }
    }
}