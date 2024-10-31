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
    public class DeleteEmployee
    {
        private readonly ILogger<DeleteEmployee> _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Constructor for <see cref="DeleteEmployee"/>.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="employeeService">The employee service to use.</param>
        public DeleteEmployee(ILogger<DeleteEmployee> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            this._employeeService = employeeService;
        }

        /// <summary>
        /// Function that handles the HTTP request to delete an employee.
        /// </summary>
        /// <param name="req">The HTTP request containing the delete employee data.</param>
        /// <returns>The HTTP response indicating the result of the delete operation.</returns>
        [Function("DeleteEmployee")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequest req)
        {
            //Log the request
            _logger.LogInformation("Handling request for deleting an employee...");
            
            //Parse the request body
            DeleteEmployeeApiRequest? body;
            try
            {
                body = JsonSerializer.Deserialize<DeleteEmployeeApiRequest>(req.Body);
            }
            catch (Exception e)
            {
                //Log the error and return an error response
                _logger.LogError(e, "Error parsing request body");
                return new BadRequestObjectResult("Invalid request body");
            }
            
            //Call the service
            DeleteEmployeeResponse response;
            try
            {
                response = await _employeeService.DeleteEmployeeAsync(body.Id);
            }
            catch (Exception e)
            {
                //Log the error and return an error response
                _logger.LogError(e, "Error deleting employee");
                return new BadRequestObjectResult("Error deleting employee");
            }
            
            //Return the response
            return new OkObjectResult(response);
        }

    }
}