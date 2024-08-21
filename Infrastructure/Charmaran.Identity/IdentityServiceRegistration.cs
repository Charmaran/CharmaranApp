using System;
using System.Text;
using Charmaran.Domain.Constants.Identity;
using Charmaran.Domain.Entities;
using Charmaran.Identity.Models;
using Charmaran.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Charmaran.Identity
{
    public static class IdentityServiceRegistration
    {
        public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddIdentity<CharmaranUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 9;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;
                })
                .AddEntityFrameworkStores<CharmaranDbContext>()
                .AddDefaultTokenProviders();
            
            
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.FromMinutes(5),
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
                    };
                });

            services.AddAuthorization(options =>
            {
                //Admin Only Policy
                options.AddPolicy(PolicyNames._adminPolicy, policy => policy.RequireRole(RoleNames._admin));
				
                //General Access Policy
                options.AddPolicy(PolicyNames._generalPolicy, policy => 
                    policy.RequireAssertion(context => context.User.IsInRole(RoleNames._admin) || context.User.IsInRole(RoleNames._user)));
            });
        }
    }
}