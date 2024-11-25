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
    /// Endpoint to create an attendance entry
    /// </summary>
    public class CreateAttendanceEntryEndpoint : Endpoint<CreateAttendanceEntryApiRequest, CreateAttendanceEntryResponse>
    {
        private readonly ILogger<CreateAttendanceEntryEndpoint> _logger;
        private readonly IAttendanceEntryService _attendanceEntryService;

        /// <summary>
        /// Constructor for <see cref="CreateAttendanceEntryEndpoint"/>.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="attendanceEntryService">The <see cref="IAttendanceEntryService"/> instance to use.</param>
        public CreateAttendanceEntryEndpoint(ILogger<CreateAttendanceEntryEndpoint> logger, IAttendanceEntryService attendanceEntryService)
        {
            this._logger = logger;
            this._attendanceEntryService = attendanceEntryService;
        }

        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access, and to listen to POST requests at
        /// <c>api/attendanceentry</c>. It is also configured to use the <c>AttendanceEntry</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Post("api/attendanceentry");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o =>
            {
                o.WithTags("AttendanceEntry");
            });
            this.Version(1);
        }

        /// <summary>
        /// Handles an incoming request to create an attendance entry.
        /// </summary>
        /// <param name="req">The <see cref="CreateAttendanceEntryApiRequest"/> containing the attendance entry to create.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method logs the request handling process and checks for the required attendance entry in the request.
        /// If the attendance entry is missing, it sends a 400 response. It then attempts to create the attendance entry using the
        /// attendance entry service and returns the appropriate response based on the success or failure of the operation.
        /// </remarks>
        public override async Task HandleAsync(CreateAttendanceEntryApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Create Attendance Entry Request");
            
            if (req.AttendanceEntry == null)
            {
                await this.SendAsync(new CreateAttendanceEntryResponse
                {
                    Success = false,
                    Message = "Attendance Entry is required"
                }, 400, cancellation: ct);
                return;
            }

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
                await this.SendAsync(createAttendanceEntryResponse, 500, cancellation: ct);
                return;
            }
            
            await this.SendAsync(createAttendanceEntryResponse, cancellation: ct);
        }
    }
}