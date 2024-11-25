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
    /// Endpoint to create an employee
    /// </summary>
    public class CreateEmployeeEndpoint : Endpoint<CreateEmployeeApiRequest, CreateEmployeeResponse>
    {
        private readonly ILogger<CreateEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Constructor for <see cref="CreateEmployeeEndpoint"/>.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="employeeService">The <see cref="IEmployeeService"/> instance to use.</param>
        public CreateEmployeeEndpoint(ILogger<CreateEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }

        /// <summary>
        /// Configures the endpoint for creating an employee.
        /// </summary>
        /// <remarks>
        /// This endpoint is configured to allow anonymous access and listens to POST requests at
        /// <c>api/employee/create</c>. It is also configured to use the <c>Employee</c> tag.
        /// </remarks>
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Post("api/employee/create");
            //this.Policies(PolicyNames._adminPolicy);
            this.Options(o => o.WithTags("Employee"));
            this.Version(1);
        }
        
        /// <summary>
        /// Handles an incoming request to create a new employee.
        /// </summary>
        /// <param name="req">The <see cref="CreateEmployeeApiRequest"/> containing the employee name to create.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method logs the request handling process and checks for the required employee name in the request.
        /// If the name is missing, it sends a 400 response. It then attempts to create a new employee using the
        /// employee service and returns the appropriate response based on the success or failure of the operation.
        /// </remarks>
        public override async Task HandleAsync(CreateEmployeeApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Create Employee Request");
            
            if (string.IsNullOrEmpty(req.Name))
            {
                await this.SendAsync(new CreateEmployeeResponse
                {
                    Success = false,
                    Message = "Name is required"
                }, 400, cancellation: ct);
                return;
            }
            
            CreateEmployeeResponse createEmployeeResponse;
            try
            {
                createEmployeeResponse = await this._employeeService.CreateEmployeeAsync(req.Name);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error creating employee");
                createEmployeeResponse = new CreateEmployeeResponse
                {
                    Success = false,
                    Message = "Unexpected Error Occurred"
                };
                await this.SendAsync(createEmployeeResponse, 500, cancellation: ct);
                return;
            }
            
            await this.SendAsync(createEmployeeResponse, cancellation: ct);
        }
    }
}