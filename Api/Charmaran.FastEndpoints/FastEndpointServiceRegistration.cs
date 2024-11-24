using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Charmaran.FastEndpoints
{
    public static class FastEndpointServiceRegistration
    {
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
					
                    options.EndpointFilter = (endpoint) => endpoint.EndpointTags == null;
                    options.AutoTagPathSegmentIndex = 0;
                });
        }
    }
}