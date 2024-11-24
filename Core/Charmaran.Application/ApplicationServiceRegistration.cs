using System.Reflection;
using Charmaran.Application.Contracts.AttendanceTracker;
using Charmaran.Application.Services.AttendanceTracker;
using Microsoft.Extensions.DependencyInjection;

namespace Charmaran.Application
{
    public static class ApplicationServiceRegistration
    {
        public static void AddAttendanceTrackerServices(this IServiceCollection services)
        {
            AddSharedServices(services);
            
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAttendanceEntryService, AttendanceEntryService>();
        }
        
        private static void AddSharedServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
        }
    }
}