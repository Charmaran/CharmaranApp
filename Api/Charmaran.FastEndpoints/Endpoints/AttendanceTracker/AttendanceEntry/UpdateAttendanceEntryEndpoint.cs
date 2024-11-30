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
    /// <summary>
    /// Endpoint to update an attendance entry
    /// </summary>
    public class UpdateAttendanceEntryEndpoint : Endpoint<UpdateAttendanceEntryApiRequest, UpdateAttendanceEntryResponse>
    {
        private readonly ILogger<UpdateAttendanceEntryEndpoint> _logger;
        private readonly IAttendanceEntryService _attendanceEntryService;

        /// <summary>
        /// Constructor for <see cref="UpdateAttendanceEntryEndpoint"/>.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="attendanceEntryService">The <see cref="IAttendanceEntryService"/> instance to use.</param>
        public UpdateAttendanceEntryEndpoint(ILogger<UpdateAttendanceEntryEndpoint> logger, IAttendanceEntryService attendanceEntryService)
        {
            this._logger = logger;
            this._attendanceEntryService = attendanceEntryService;
        }
        
        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access, and to listen to PUT requests at
        /// <c>api/attendanceentry</c>. It is also configured to use the <c>AttendanceEntry</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Put("api/attendanceentry");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("AttendanceEntry"));
        }
        
        /// <summary>
        /// Handles an incoming request to update an attendance entry.
        /// </summary>
        /// <param name="req">The <see cref="UpdateAttendanceEntryApiRequest"/> containing the attendance entry to update.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method logs the request handling process and checks for the required attendance entry in the request.
        /// If the attendance entry is missing, it sends a 400 response. It then attempts to update the attendance entry using the
        /// attendance entry service and returns the appropriate response based on the success or failure of the operation.
        /// </remarks>
        public override async Task HandleAsync(UpdateAttendanceEntryApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Update Attendance Entry Request");
            
            if (req.AttendanceEntry == null)
            {
                await this.SendAsync(new UpdateAttendanceEntryResponse
                {
                    Success = false,
                    Message = "Attendance Entry is required"
                }, 400, cancellation: ct);
                return;
            }

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
                await this.SendAsync(updateAttendanceEntryResponse, 500, cancellation: ct);
                return;
            }
            
            await this.SendAsync(updateAttendanceEntryResponse, cancellation: ct);
        }
    }
}