using System;
using Charmaran.UI.Contracts;
using Charmaran.UI.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Charmaran.UI
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSecurityServices(configuration);
        }

        // Add Security Services using an example found at 
        //https://github.com/dotnet/blazor-samples/tree/main/9.0/BlazorWebAssemblyStandaloneWithIdentity
        private static void AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
            services.AddTransient<AuthHeaderHandler>();
            
            services.AddHttpClient(
                    "Auth",
                    opt => opt.BaseAddress = new Uri(configuration["ApiUrl"] ?? "https://localhost:5032"))
                .AddHttpMessageHandler<AuthHeaderHandler>();
        }
    }
}