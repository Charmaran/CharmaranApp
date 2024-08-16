using System;
using Charmaran.UI.Contracts.Refit;
using Charmaran.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Charmaran.UI
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            AddViewModels(services);
            AddRefit(services);
        }

        public static void AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<MainWindowViewModel>();
        }

        private static void AddRefit(this IServiceCollection services)
        {
            services.AddRefitClient<IAuthenticationApiService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri("https://localhost:5000/api");
                });
        }
    }
}