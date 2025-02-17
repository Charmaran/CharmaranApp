using Charmaran.Domain.Constants.Identity;
using Charmaran.Domain.Entities;
using Charmaran.FastEndpoints;
using Charmaran.Persistence;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Http;
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
                    opts.WithOrigins("https://localhost:5168", "http://localhost:5032", "http://localhost:5132", "https://charmaran.codesmithing.io")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
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
            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddIdentityCookies();
            
            // The default values, which are appropriate for hosting the Backend and
            // BlazorWasmAuth apps on the same domain, are Lax and SameAsRequest. 
            // https://learn.microsoft.com/aspnet/core/blazor/security/webassembly/standalone-with-identity#cross-domain-hosting-same-site-configuration
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            services.AddAuthorizationBuilder()
                .AddPolicy(PolicyNames._adminPolicy, policy => policy.RequireRole(RoleNames._admin))
                .AddPolicy(PolicyNames._generalPolicy, policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole(RoleNames._admin) || context.User.IsInRole(RoleNames._user)));

            services.AddIdentityCore<CharmaranUser>(options =>
                {
                    options.Password.RequiredLength = 9;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<CharmaranDbContext>()
                .AddApiEndpoints();
        }
    }
}