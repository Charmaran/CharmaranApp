using System;
using System.Text.Json;
using System.Threading.Tasks;
using Charmaran.Application.Contracts.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Requests;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace Charmaran.CreateEmployee
{
    public class CreateEmployee
    {
        private readonly ILogger<CreateEmployee> _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Creates a new instance of the <see cref="CreateEmployee"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="employeeService">The employee service to use.</param>
        public CreateEmployee(ILogger<CreateEmployee> logger, IEmployeeService employeeService)
        {
            // Set the logger for the class
            _logger = logger;
            this._employeeService = employeeService;
        }

        /// <summary>
        /// Function that creates a new employee.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <returns>The HTTP response.</returns>
        [Function("CreateEmployee")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            // Log the request
            _logger.LogInformation("Handling request for creating a new employee...");
            
            // Parse the request body
            CreateEmployeeApiRequest? body;
            try
            {
                body = JsonSerializer.Deserialize<CreateEmployeeApiRequest>(req.Body);  
            }
            catch (Exception e)
            {
                // Log the error and return an error response
                this._logger.LogError(e, "Error parsing request body");
                return new BadRequestObjectResult("Invalid request body");
            }
            
            // Call the service
            CreateEmployeeResponse response = await _employeeService.CreateEmployeeAsync(body?.Name ?? string.Empty);
            
            // Return the result
            return new OkObjectResult(response);
        }

    }
}