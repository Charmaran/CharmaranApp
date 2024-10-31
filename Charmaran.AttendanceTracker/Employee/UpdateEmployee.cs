using System.Text.Json;
using Charmaran.Application.Contracts.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Requests.Employee;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Charmaran.AttendanceTracker.Employee
{
    public class UpdateEmployee
    {
        private readonly ILogger<UpdateEmployee> _logger;
        private readonly IEmployeeService _employeeService;

        
        /// <summary>
        /// Constructor for <see cref="UpdateEmployee"/>.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="employeeService">The employee service to use.</param>
        public UpdateEmployee(ILogger<UpdateEmployee> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            this._employeeService = employeeService;
        }

        [Function("UpdateEmployee")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequest req)
        {
            //Log the request
            _logger.LogInformation("Handling request for updating an employee...");
            
            //Parse the request body
            UpdateEmployeeApiRequest? body;
            try
            {
                body = JsonSerializer.Deserialize<UpdateEmployeeApiRequest>(req.Body);
            }
            catch (Exception e)
            {
                //Log the error and return an error response
                _logger.LogError(e, "Error parsing request body");
                return new BadRequestObjectResult("Invalid request body");
            }
            
            //Call the service
            UpdateEmployeeResponse response = await this._employeeService.UpdateEmployeeAsync(body?.Id ?? 0, body?.Name ?? string.Empty);
            
            //Return the result
            return new OkObjectResult(response);
        }

    }
}