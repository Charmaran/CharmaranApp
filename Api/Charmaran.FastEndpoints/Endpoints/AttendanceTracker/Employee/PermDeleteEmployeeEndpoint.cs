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
    /// Endpoint to permanently delete an employee
    /// </summary>
    public class PermDeleteEmployeeEndpoint : Endpoint<PermanentDeleteEmployeeApiRequest, PermanentDeleteEmployeeResponse>
    {
        private readonly ILogger<PermDeleteEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Constructor for <see cref="PermDeleteEmployeeEndpoint"/>.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="employeeService">The <see cref="IEmployeeService"/> instance to use.</param>
        public PermDeleteEmployeeEndpoint(ILogger<PermDeleteEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }
        
        /// <summary>
        /// Configures the endpoint to permanently delete an employee.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access and listens to DELETE requests at
        /// <c>api/employee/delete</c>. It is also configured to use the <c>Employee</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Delete("api/employee/delete");
            //this.Policies(PolicyNames._adminPolicy);
            this.Options(o => o.WithTags("Employee"));
            this.Version(1);
        }

        /// <summary>
        /// Handles an incoming request to permanently delete an employee.
        /// </summary>
        /// <param name="req">The <see cref="PermanentDeleteEmployeeApiRequest"/> containing the employee id to delete.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method logs the request handling process and attempts to permanently delete the employee using the
        /// employee service and returns the appropriate response based on the success or failure of the operation.
        /// </remarks>
        public override async Task HandleAsync(PermanentDeleteEmployeeApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Permanent Delete Employee Request");

            PermanentDeleteEmployeeResponse permanentDeleteEmployeeResponse;
            try
            {
                permanentDeleteEmployeeResponse = await this._employeeService.PermanentDeleteEmployeeAsync(req.Id);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error permanently deleting employee");
                permanentDeleteEmployeeResponse = new PermanentDeleteEmployeeResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
                await this.SendAsync(permanentDeleteEmployeeResponse, 500, cancellation: ct);
                return;
            }
            
            await this.SendAsync(permanentDeleteEmployeeResponse, cancellation: ct);
        }
    }
}