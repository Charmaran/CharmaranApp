using System;
using Charmaran.Application;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Charmaran.FastEndpoints
{
    /// <summary>
    /// Service registration for the FastEndpoints library
    /// </summary>
    public static class FastEndpointServiceRegistration
    {
        /// <summary>
        /// Registers services for the FastEndpoints library and configures Swagger for the API.
        /// </summary>
        /// <param name="services">The service collection to add the services to.</param>
        /// <param name="configuration">The configuration for the services.</param>
        public static void AddFastEndpointServices(this IServiceCollection services, IConfiguration configuration)
        {
            //https://fast-endpoints.com/
            services.AddFastEndpoints()
                .SwaggerDocument(options =>
                {
                    options.DocumentSettings = s =>
                    {
                        s.Title = "Charmaran API";
                        s.Version = "v1";
                        s.Description = "API documentation for the Charmaran application";
                    };

                    options.EndpointFilter = new Func<EndpointDefinition, bool>(ep => ep.EndpointTags == null);
                    options.AutoTagPathSegmentIndex = 0;
                });
            
            services.AddAttendanceTrackerServices(configuration);
        }
    }
}