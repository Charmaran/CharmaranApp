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
    public class UpdateEmployeeEndpoint : Endpoint<UpdateEmployeeApiRequest, UpdateEmployeeResponse>
    {
        private readonly ILogger<UpdateEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        public UpdateEmployeeEndpoint(ILogger<UpdateEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }

        public override void Configure()
        {
            this.AllowAnonymous();
            this.Put("api/employee/update");
            //this.Policies(PolicyNames._adminPolicy);
            this.Options(o => o.WithTags("Employee"));
            this.Version(1);
        }
        
        public override async Task HandleAsync(UpdateEmployeeApiRequest req, CancellationToken ct)
        {
            this._logger.LogInformation("Handling Update Employee Request");

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
            }
            
            await this.SendAsync(updateEmployeeResponse, cancellation: ct);
        }
    }
}