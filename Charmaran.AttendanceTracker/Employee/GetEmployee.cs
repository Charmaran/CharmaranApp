using Charmaran.Application.Contracts.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker.Responses.Employee;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Charmaran.AttendanceTracker.Employee
{
    public class GetEmployee
    {
        private readonly ILogger<GetEmployee> _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmployee"/> class.
        /// </summary>
        /// <param name="logger">The logger to use for logging operations.</param>
        /// <param name="employeeService">The service used to interact with employee data.</param>
        public GetEmployee(ILogger<GetEmployee> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            this._employeeService = employeeService;
        }

        /// <summary>
        /// Handles the HTTP request to retrieve an employee.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <returns>The HTTP response containing the employee.</returns>
        [Function("GetEmployee")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            // Log the request
            _logger.LogInformation("Handling request for retrieving an employee...");
            
            // Parse the request for the Id
            string? id = req.Query["Id"];

            // Validate the id is not null or empty
            if (string.IsNullOrEmpty(id))
            {
                this._logger.LogWarning("No id provided, returning bad request");
                return new BadRequestObjectResult("Id is required");
            }
            
            // Call the service
            GetEmployeeResponse response;
            try
            {
                response = await this._employeeService.GetEmployeeByIdAsync(int.Parse(id));
            }
            catch (Exception e)
            {
                // Log the error and return an error response
                _logger.LogError(e, "Error retrieving employee");
                return new BadRequestObjectResult("Error retrieving employee");
            }
            
            // Return the response
            return new OkObjectResult(response);
        }

    }
}