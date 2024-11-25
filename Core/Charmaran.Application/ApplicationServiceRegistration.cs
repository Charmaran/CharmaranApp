using System.Reflection;
using Charmaran.Application.Contracts.AttendanceTracker;
using Charmaran.Application.Services.AttendanceTracker;
using Charmaran.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Charmaran.Application
{
    public static class ApplicationServiceRegistration
    {
        public static void AddAttendanceTrackerServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddSharedServices(services, configuration);
            
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAttendanceEntryService, AttendanceEntryService>();
        }
        
        private static void AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            services.AddPersistenceServices(configuration);
        }
    }
}