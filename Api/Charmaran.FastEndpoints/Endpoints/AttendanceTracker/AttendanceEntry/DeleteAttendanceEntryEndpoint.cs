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
    /// Endpoint to delete an attendance entry
    /// </summary>
    public class DeleteAttendanceEntryEndpoint : Endpoint<DeleteAttendanceEntryApiRequest, DeleteAttendanceEntryResponse>
    {
        private readonly ILogger<DeleteAttendanceEntryEndpoint> _logger;
        private readonly IAttendanceEntryService _attendanceEntryService;

        /// <summary>
        /// Constructor for <see cref="DeleteAttendanceEntryEndpoint"/>.
        /// </summary>
        /// <param name="logger">The logger to use for logging information and errors.</param>
        /// <param name="attendanceEntryService">The service used to interact with attendance entries.</param>
        public DeleteAttendanceEntryEndpoint(ILogger<DeleteAttendanceEntryEndpoint> logger, IAttendanceEntryService attendanceEntryService)
        {
            this._logger = logger;
            this._attendanceEntryService = attendanceEntryService;
        }
        
        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access, and to listen to DELETE requests at
        /// <c>api/attendanceentry</c>. It is also configured to use the <c>AttendanceEntry</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Delete("api/attendanceentry");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("AttendanceEntry"));
        }

        /// <summary>
        /// Handles an incoming request to delete an attendance entry.
        /// </summary>
        /// <param name="req">The <see cref="DeleteAttendanceEntryApiRequest"/> containing the id of the attendance entry to delete.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method logs the request handling process and attempts to delete the attendance entry using the attendance entry service.
        /// If the operation fails, it logs the error and returns a response containing an error message.
        /// Otherwise, it returns a response containing the result of the operation.
        /// </remarks>
        public override async Task HandleAsync(DeleteAttendanceEntryApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Delete Attendance Entry Request");

            DeleteAttendanceEntryResponse deleteAttendanceEntryResponse;
            try
            {
                deleteAttendanceEntryResponse = await this._attendanceEntryService.DeleteAttendanceEntryAsync(req.Id);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error deleting attendance entry");
                deleteAttendanceEntryResponse = new DeleteAttendanceEntryResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
                await this.SendAsync(deleteAttendanceEntryResponse, 500, cancellation: ct);
                return;
            }
            
            await this.SendAsync(deleteAttendanceEntryResponse, cancellation: ct);
        }
    }
}