using System;
using System.Threading;
using System.Threading.Tasks;
using Charmaran.Application.Contracts.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Charmaran.FastEndpoints.Endpoints.AttendanceTracker.Employee
{
    /// <summary>
    /// Endpoint to get all employees
    /// </summary>
    public class GetEmployeesEndpoint : Endpoint<EmptyRequest, GetAllEmployeesResponse>
    {
        private readonly ILogger<GetEmployeesEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Constructor for <see cref="GetEmployeesEndpoint"/>.
        /// </summary>
        /// <param name="logger">The logger to use for logging information and errors.</param>
        /// <param name="employeeService">The service used to interact with employees.</param>
        public GetEmployeesEndpoint(ILogger<GetEmployeesEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }
        
        /// <summary>
        /// Configures the endpoint for getting all employees.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access and listens to GET requests at
        /// <c>api/employees/all</c>. It is also configured to use the <c>Employee</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Get("api/employees/all");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("Employee"));
        }

        /// <summary>
        /// Handles an incoming request to get all employees.
        /// </summary>
        /// <param name="req">The <see cref="EmptyRequest"/> representing the request.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method logs the request handling process and attempts to retrieve all employees using the employee service.
        /// If the operation fails, it logs the error and returns a response containing an error message.
        /// Otherwise, it returns a response containing the list of employees.
        /// </remarks>
        public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Get Employees Request");
            
            GetAllEmployeesResponse getAllEmployeesResponse;
            try
            {
                getAllEmployeesResponse = await this._employeeService.GetEmployeesAsync();
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error getting employees");
                getAllEmployeesResponse = new GetAllEmployeesResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
                await this.SendAsync(getAllEmployeesResponse, 500, cancellation: ct);
                return;
            }
            
            await this.SendAsync(getAllEmployeesResponse, cancellation: ct);
        }
    }
}