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
    /// Endpoint to get all attendance entries for an employee
    /// </summary>
    public class GetEmployeeAttendanceEntriesEndpoint : Endpoint<GetEmployeeAttendanceEntriesApiRequest, GetEmployeeAttendanceEntriesResponse>
    {
        private readonly ILogger<GetEmployeeAttendanceEntriesEndpoint> _logger;
        private readonly IAttendanceEntryService _attendanceEntryService;

        /// <summary>
        /// Constructor for <see cref="GetEmployeeAttendanceEntriesEndpoint"/>.
        /// </summary>
        /// <param name="logger">The logger to use for logging information and errors.</param>
        /// <param name="attendanceEntryService">The service used to interact with attendance entries.</param>
        public GetEmployeeAttendanceEntriesEndpoint(ILogger<GetEmployeeAttendanceEntriesEndpoint> logger, IAttendanceEntryService attendanceEntryService)
        {
            this._logger = logger;
            this._attendanceEntryService = attendanceEntryService;
        }
        
        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access, and to listen to GET requests at
        /// <c>api/attendanceentry</c>. It is also configured to use the <c>AttendanceEntry</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Get("api/attendanceentry");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("AttendanceEntry"));
        }
        
        /// <summary>
        /// Handles an incoming request to get all attendance entries for an employee.
        /// </summary>
        /// <param name="req">The <see cref="GetEmployeeAttendanceEntriesApiRequest"/> containing the employee id to get attendance entries for.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method logs the request handling process and attempts to get all attendance entries for the specified employee id using the attendance entry service.
        /// If the operation fails, it logs the error and returns a response containing an error message.
        /// Otherwise, it returns a response containing the attendance entries.
        /// </remarks>
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
                await this.SendAsync(getEmployeeAttendanceEntriesResponse, 500, cancellation: ct);
                return;
            }
            
            await this.SendAsync(getEmployeeAttendanceEntriesResponse, cancellation: ct);
        }
    }
}