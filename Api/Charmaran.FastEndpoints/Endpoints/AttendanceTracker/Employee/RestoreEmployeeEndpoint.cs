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
    public class RestoreEmployeeEndpoint : Endpoint<RestoreEmployeeApiRequest, RestoreEmployeeResponse>
    {
        private readonly ILogger<RestoreEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        public RestoreEmployeeEndpoint(ILogger<RestoreEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }
        
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Put("api/employee/restore");
            this.Options(o => o.WithTags("Employee"));
            this.Version(1);
        }

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
            }
            
            await this.SendAsync(restoreEmployeeResponse, cancellation: ct);
        }
    }
}