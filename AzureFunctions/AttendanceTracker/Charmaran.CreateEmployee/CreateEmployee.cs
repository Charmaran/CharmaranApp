using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Charmaran.CreateEmployee
{
    public class CreateEmployee
    {
        private readonly ILogger<CreateEmployee> _logger;

        /// <summary>
        /// Creates a new instance of the <see cref="CreateEmployee"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public CreateEmployee(ILogger<CreateEmployee> logger)
        {
            // Set the logger for the class
            _logger = logger;
        }

        /// <summary>
        /// Function that creates a new employee.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <returns>The HTTP response.</returns>
        [Function("CreateEmployee")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            // TODO: Implement the logic to create a new employee
            return new OkObjectResult("Welcome to Azure Functions!");
        }

    }
}