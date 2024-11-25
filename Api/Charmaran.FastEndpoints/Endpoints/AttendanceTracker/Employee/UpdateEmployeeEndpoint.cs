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
    /// Endpoint to update an employee
    /// </summary>
    public class UpdateEmployeeEndpoint : Endpoint<UpdateEmployeeApiRequest, UpdateEmployeeResponse>
    {
        private readonly ILogger<UpdateEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Constructor for <see cref="UpdateEmployeeEndpoint"/>.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="employeeService">The <see cref="IEmployeeService"/> instance to use.</param>
        public UpdateEmployeeEndpoint(ILogger<UpdateEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }

        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access, and to listen to PUT requests at
        /// <c>api/employee/update</c>. It is also configured to use the <c>Employee</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Put("api/employee/update");
            //this.Policies(PolicyNames._adminPolicy);
            this.Options(o => o.WithTags("Employee"));
            this.Version(1);
        }
        
        /// <summary>
        /// Handles an incoming request to update an employee.
        /// </summary>
        /// <param name="req">The <see cref="UpdateEmployeeApiRequest"/> containing the employee id and name to update.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access, and to listen to PUT requests at
        /// <c>api/employee/update</c>. It is also configured to use the <c>Employee</c> tag.
        /// </remarks>
        public override async Task HandleAsync(UpdateEmployeeApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Update Employee Request");
            
            if (string.IsNullOrEmpty(req.Name) || req.Id == 0)
            {
                await this.SendAsync(new UpdateEmployeeResponse
                {
                    Success = false,
                    Message = "Name and Id are required"
                }, 400, cancellation: ct);
                return;
            }

            UpdateEmployeeResponse updateEmployeeResponse;
            try
            {
                updateEmployeeResponse = await this._employeeService.UpdateEmployeeAsync(req.Id, req.Name);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error updating employee");
                updateEmployeeResponse = new UpdateEmployeeResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
                await this.SendAsync(updateEmployeeResponse, 500, cancellation: ct);
                return;
            }
            
            await this.SendAsync(updateEmployeeResponse, cancellation: ct);
        }
    }
}