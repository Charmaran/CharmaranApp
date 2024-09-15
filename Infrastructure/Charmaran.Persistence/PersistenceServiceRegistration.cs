using Charmaran.Persistence.Contracts.AttendanceTracker;
using Charmaran.Persistence.Repositories.AttendanceTracker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Charmaran.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Register the DbContext
            services.AddDbContext<CharmaranDbContext>(options =>
            {
                options.UseCosmos(configuration["CosmosDB:Endpoint"] ?? string.Empty, 
                    configuration["CosmosDB:PrimaryKey"] ?? string.Empty,
                    configuration["CosmosDB:Database"] ?? string.Empty);
            });

            //Register the repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAttendanceEntryRepository, AttendanceEntryRepository>();
            
        }
    }
}