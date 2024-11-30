using System;
using System.Threading;
using System.Threading.Tasks;
using Charmaran.Application.Contracts.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Requests.Employee;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Charmaran.FastEndpoints.Endpoints.AttendanceTracker.Employee
{
    /// <summary>
    /// Endpoint to restore an employee
    /// </summary>
    public class RestoreEmployeeEndpoint : Endpoint<RestoreEmployeeApiRequest, RestoreEmployeeResponse>
    {
        private readonly ILogger<RestoreEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Constructor for <see cref="RestoreEmployeeEndpoint"/>.
        /// </summary>
        /// <param name="logger">The logger to use for logging information and errors.</param>
        /// <param name="employeeService">The service used to interact with employees.</param>
        public RestoreEmployeeEndpoint(ILogger<RestoreEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }
        
        /// <summary>
        /// Configures the endpoint to restore an employee.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access and listens to PUT requests at
        /// <c>api/employee/restore</c>. It is also configured to use the <c>Employee</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Put("api/employee/restore");
            this.Options(o => o.WithTags("Employee"));
        }

        /// <summary>
        /// Handles an incoming request to restore an employee.
        /// </summary>
        /// <param name="request">The <see cref="RestoreEmployeeApiRequest"/> containing the employee id to restore.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method logs the request handling process and attempts to restore the employee using the employee service.
        /// If the operation fails, it logs the error and returns a response containing an error message.
        /// Otherwise, it returns a response containing the result of the operation.
        /// </remarks>
        public override async Task HandleAsync(RestoreEmployeeApiRequest request, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Restore Employee Request");

            RestoreEmployeeResponse restoreEmployeeResponse;
            try
            {
                restoreEmployeeResponse = await this._employeeService.RestoreEmployeeAsync(request.Id);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error restoring employee");
                restoreEmployeeResponse = new RestoreEmployeeResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
                await this.SendAsync(restoreEmployeeResponse, cancellation: ct);
                return;
            }
            
            await this.SendAsync(restoreEmployeeResponse, cancellation: ct);
        }
    }
}