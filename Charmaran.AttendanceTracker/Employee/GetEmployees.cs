using Charmaran.Application.Contracts.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace Charmaran.AttendanceTracker.Employee
{
    public class GetEmployees
    {
        private readonly ILogger<GetEmployees> _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Constructor for the <see cref="GetEmployees"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="employeeService">The employee service to use.</param>
        public GetEmployees(ILogger<GetEmployees> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            this._employeeService = employeeService;
        }

        /// <summary>
        /// Function that handles the HTTP request to retrieve all employees.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <returns>The HTTP response containing the employees.</returns>
        [Function("GetEmployees")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            //Log the request
            _logger.LogInformation("Handling request for retrieving all employees...");
            
            //Call the service
            GetAllEmployeesResponse response;
            try
            {
                response = await this._employeeService.GetEmployeesAsync();
            }
            catch (Exception e)
            {
                //Log the error and return an error response
                _logger.LogError(e, "Error retrieving employees");
                return new BadRequestObjectResult("Error retrieving employees");
            }
            
            //Return the response
            return new OkObjectResult(response);
            
        }

    }
}