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
    public class DeleteEmployeeEndpoint : Endpoint<DeleteEmployeeApiRequest, DeleteEmployeeResponse>
    {
        private readonly ILogger<DeleteEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        public DeleteEmployeeEndpoint(ILogger<DeleteEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }
        
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Delete("api/employee");
            //this.Policies(PolicyNames._adminPolicy);
            this.Options(o => o.WithTags("Employee"));
            this.Version(1);
        }
        
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
            }
            
            await this.SendAsync(deleteEmployeeResponse, cancellation: ct);
        }
    }
}