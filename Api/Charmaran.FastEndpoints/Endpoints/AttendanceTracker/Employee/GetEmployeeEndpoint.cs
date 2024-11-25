using System;
using System.Threading;
using System.Threading.Tasks;
using Charmaran.Application.Contracts.AttendanceTracker;
using Charmaran.Domain.Constants.Identity;
using Charmaran.Shared.AttendanceTracker.Requests.Employee;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Charmaran.FastEndpoints.Endpoints.AttendanceTracker.Employee
{
    /// <summary>
    /// Endpoint to get an employee
    /// </summary>
    public class GetEmployeeEndpoint : Endpoint<GetEmployeeApiRequest, GetEmployeeResponse>
    {
        private readonly ILogger<GetEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Constructor for <see cref="GetEmployeeEndpoint"/>.
        /// </summary>
        /// <param name="logger">The logger to use for logging information and errors.</param>
        /// <param name="employeeService">The service used to interact with employees.</param>
        public GetEmployeeEndpoint(ILogger<GetEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }
        
        /// <summary>
        /// Configures the endpoint for getting an employee.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access and listens to GET requests at
        /// <c>api/employee/</c>. It is also configured to use the <c>Employee</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Get("api/employee/");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("Employee"));
            this.Version(1);
        }
        
        /// <summary>
        /// Handles an incoming request to get an employee.
        /// </summary>
        /// <param name="request">The <see cref="GetEmployeeApiRequest"/> containing the employee id to get.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method logs the request handling process and attempts to get the employee using the employee service and returns the appropriate response based on the success or failure of the operation.
        /// </remarks>
        public override async Task HandleAsync(GetEmployeeApiRequest request, CancellationToken cancellationToken)
        {
            this._logger.LogInformation("Handling Get Employee Request");

            GetEmployeeResponse getEmployeeResponse;
            try
            {
                getEmployeeResponse = await this._employeeService.GetEmployeeByIdAsync(request.Id);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error getting employee");
                getEmployeeResponse = new GetEmployeeResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
                await this.SendAsync(getEmployeeResponse, 500, cancellation: cancellationToken);
                return;
            }
            
            await this.SendAsync(getEmployeeResponse, cancellation: cancellationToken);
        }
    }
}