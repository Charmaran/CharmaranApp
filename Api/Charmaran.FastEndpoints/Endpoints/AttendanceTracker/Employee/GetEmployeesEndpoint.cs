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
    public class GetEmployeesEndpoint : Endpoint<EmptyRequest, GetAllEmployeesResponse>
    {
        private readonly ILogger<GetEmployeesEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        public GetEmployeesEndpoint(ILogger<GetEmployeesEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }
        
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Get("api/employees/all");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("Employee"));
            this.Version(1);
        }

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
            }
            
            await this.SendAsync(getAllEmployeesResponse, cancellation: ct);
        }
    }
}