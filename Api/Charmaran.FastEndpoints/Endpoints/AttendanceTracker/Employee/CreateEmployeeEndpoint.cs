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
    public class CreateEmployeeEndpoint : Endpoint<CreateEmployeeApiRequest, CreateEmployeeResponse>
    {
        private readonly ILogger<CreateEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        public CreateEmployeeEndpoint(ILogger<CreateEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }

        public override void Configure()
        {
            this.AllowAnonymous();
            this.Post("api/employee/create");
            //this.Policies(PolicyNames._adminPolicy);
            this.Options(o => o.WithTags("Employee"));
        }
        
        public override async Task HandleAsync(CreateEmployeeApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Create Employee Request");
            
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
            }
            
            await this.SendAsync(createEmployeeResponse, cancellation: ct);
        }
    }
}