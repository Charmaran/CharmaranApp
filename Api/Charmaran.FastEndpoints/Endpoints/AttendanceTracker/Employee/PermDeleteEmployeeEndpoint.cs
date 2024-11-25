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
    public class PermDeleteEmployeeEndpoint : Endpoint<PermanentDeleteEmployeeApiRequest, PermanentDeleteEmployeeResponse>
    {
        private readonly ILogger<PermDeleteEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        public PermDeleteEmployeeEndpoint(ILogger<PermDeleteEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }
        
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Delete("api/employee/delete");
            //this.Policies(PolicyNames._adminPolicy);
            this.Options(o => o.WithTags("Employee"));
            this.Version(1);
        }

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
            }
            
            await this.SendAsync(permanentDeleteEmployeeResponse, cancellation: ct);
        }
    }
}