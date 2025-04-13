using System;
using System.Net.Http;
using Blazored.Modal;
using Blazored.Toast;
using Charmaran.UI.Contracts;
using Charmaran.UI.Contracts.Identity;
using Charmaran.UI.Contracts.Refit;
using Charmaran.UI.Identity;
using Charmaran.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Charmaran.UI
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSecurityServices(configuration);
            services.AddRefitServices(configuration);

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAttendanceEntryService, AttendanceEntryService>();
            
            // Blazored Toast https://github.com/Blazored/Toast
            services.AddBlazoredToast();
            
            // Blazored Modal https://github.com/Blazored/Modal
            services.AddBlazoredModal();
        }
        
        // Add Security Services using an example found at
        // https://github.com/dotnet/blazor-samples/tree/main/9.0/BlazorWebAssemblyStandaloneWithIdentity
        // https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/standalone-with-identity/?view=aspnetcore-9.0
        private static void AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
            services.AddTransient<AuthHeaderHandler>();
            
            services.AddScoped(_ =>
                new HttpClient { BaseAddress = new Uri(configuration["FrontendUrl"] ?? "https://localhost:5132") });
            
            services.AddHttpClient(
                    "Auth",
                    opt => opt.BaseAddress = new Uri(configuration["ApiUrl"] ?? "http://localhost:5032"))
                .AddHttpMessageHandler<AuthHeaderHandler>();
        }
        
        //https://github.com/reactiveui/refit
        private static void AddRefitServices(this IServiceCollection services, IConfiguration configuration)
        {
            string apiEndpoint = configuration["ApiUrl"] ?? "http://localhost:5032";

            RefitSettings refitSettings = new RefitSettings(new NewtonsoftJsonContentSerializer());
            
            services.AddRefitClient<IEmployeeApiService>(refitSettings)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(apiEndpoint);
                }).AddHttpMessageHandler<AuthHeaderHandler>();
            
            services.AddRefitClient<IAttendanceEntryApiService>(refitSettings)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(apiEndpoint);
                }).AddHttpMessageHandler<AuthHeaderHandler>();
        }
    }
}