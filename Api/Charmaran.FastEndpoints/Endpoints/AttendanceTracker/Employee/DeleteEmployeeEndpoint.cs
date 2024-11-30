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
    /// Endpoint to delete an employee
    /// </summary>
    public class DeleteEmployeeEndpoint : Endpoint<DeleteEmployeeApiRequest, DeleteEmployeeResponse>
    {
        private readonly ILogger<DeleteEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Constructor for <see cref="DeleteEmployeeEndpoint"/>.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="employeeService">The <see cref="IEmployeeService"/> instance to use.</param>
        public DeleteEmployeeEndpoint(ILogger<DeleteEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }
        
        /// <summary>
        /// Configures the endpoint for deleting an employee.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access and listens to DELETE requests at
        /// <c>api/employee</c>. It is also configured to use the <c>Employee</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Delete("api/employee");
            //this.Policies(PolicyNames._adminPolicy);
            this.Options(o => o.WithTags("Employee"));
        }
        
        /// <summary>
        /// Handles an incoming request to delete an employee.
        /// </summary>
        /// <param name="req">The <see cref="DeleteEmployeeApiRequest"/> containing the employee id to delete.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method logs the request handling process and attempts to delete the employee using the
        /// employee service and returns the appropriate response based on the success or failure of the operation.
        /// </remarks>
        public override async Task HandleAsync(DeleteEmployeeApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Delete Employee Request");

            DeleteEmployeeResponse deleteEmployeeResponse;
            try
            {
                deleteEmployeeResponse = await this._employeeService.DeleteEmployeeAsync(req.Id);
            }
            catch (Exception e) {
                this._logger.LogError(e, "Error deleting employee");
                deleteEmployeeResponse = new DeleteEmployeeResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
                await this.SendAsync(deleteEmployeeResponse, 500, cancellation: ct);
                return;
            }
            
            await this.SendAsync(deleteEmployeeResponse, cancellation: ct);
        }
    }
}