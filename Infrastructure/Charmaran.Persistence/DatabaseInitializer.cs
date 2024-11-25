using System;
using Charmaran.Domain.Constants.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Charmaran.Persistence
{
    public static class DatabaseInitializer
    {
        public static void MigrateDatabase(CharmaranDbContext dbContext)
        {
            dbContext.Database.Migrate();
        }

        public static void PostMigrationUpdates(CharmaranDbContext dbContext, RoleManager<IdentityRole<Guid>> roleManager)
        {
            // Add roles to the database
            if (roleManager.RoleExistsAsync(RoleNames._admin).Result == false)
            {
                IdentityRole<Guid> role = new IdentityRole<Guid>
                {
                    Name = RoleNames._admin
                };
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;

                if (roleResult.Succeeded == false)
                {
                    Console.WriteLine("Failed to create the admin role.");
                }
            }

            if (roleManager.RoleExistsAsync(RoleNames._user).Result == false)
            {
                IdentityRole<Guid> role = new IdentityRole<Guid>
                {
                    Name = RoleNames._user
                };
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                
                if (roleResult.Succeeded == false)
                {
                    Console.WriteLine("Failed to create the user role.");
                }
            }
        }
    }
}