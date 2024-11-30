using Charmaran.Domain.Constants.Identity;
using Charmaran.Domain.Entities;
using Charmaran.FastEndpoints;
using Charmaran.Persistence;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Charmaran.Api
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddCors(policy =>
            {
                policy.AddPolicy("CorsPolicy", opts =>
                    opts.WithOrigins(["http://localhost:5168", "http://localhost:5168", "https://charmaran.codesmithing.io"])
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
            });

            // Add Data Protection
            //https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-8.0#changing-algorithms-with-usecryptographicalgorithms
            services.AddDataProtection()
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });


            //Add Services
            services.AddControllers();
            services.AddFastEndpointServices(configuration);

            //Add Identity
            services.AddSecurity();
        }
        
        private static void AddSecurity(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                //Admin Only Policy
                options.AddPolicy(PolicyNames._adminPolicy, policy => policy.RequireRole(RoleNames._admin));

                //General Access Policy
                options.AddPolicy(PolicyNames._generalPolicy, policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole(RoleNames._admin) || context.User.IsInRole(RoleNames._user)));
            });

            services.AddIdentity<CharmaranUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 9;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;
                })
                .AddEntityFrameworkStores<CharmaranDbContext>()
                .AddDefaultTokenProviders()
                .AddApiEndpoints();
        }
    }
}