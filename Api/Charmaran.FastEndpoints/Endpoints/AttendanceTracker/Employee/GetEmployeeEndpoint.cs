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
    public class GetEmployeeEndpoint : Endpoint<GetEmployeeApiRequest, GetEmployeeResponse>
    {
        private readonly ILogger<GetEmployeeEndpoint> _logger;
        private readonly IEmployeeService _employeeService;

        public GetEmployeeEndpoint(ILogger<GetEmployeeEndpoint> logger, IEmployeeService employeeService)
        {
            this._logger = logger;
            this._employeeService = employeeService;
        }
        
        public override void Configure()
        {
            this.AllowAnonymous();
            this.Get("api/employee/");
            //this.Policies(PolicyNames._generalPolicy);
            this.Options(o => o.WithTags("Employee"));
        }
        
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
            }
            
            await this.SendAsync(getEmployeeResponse, cancellation: cancellationToken);
        }
    }
}