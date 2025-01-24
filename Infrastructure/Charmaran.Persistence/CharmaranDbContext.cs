using Charmaran.Domain.Entities;
using Charmaran.Domain.Entities.AttendanceTracker;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Charmaran.Persistence
{
    public class CharmaranDbContext(DbContextOptions<CharmaranDbContext> options) : IdentityDbContext<CharmaranUser>(options)
    {
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        
        public virtual DbSet<AttendanceEntry> AttendanceEntries { get; set; } = null!;
        
        public virtual DbSet<Holiday> Holidays { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            //-----------------------------------------------------------------------------------------------------------
            // Employee
            //-----------------------------------------------------------------------------------------------------------
            builder.Entity<Employee>()
                .ToTable(t =>
                    {
                        t.HasCheckConstraint(
                            "CHK_ELastModifiedDate",
                            "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate"
                        );

                        t.HasCheckConstraint(
                            "CHK_ECreatedDate",
                            "CreatedDate <= LastModifiedDate"
                        );
                    }
                );
            
            //-----------------------------------------------------------------------------------------------------------
            // AttendanceEntry
            //-----------------------------------------------------------------------------------------------------------
            builder.Entity<AttendanceEntry>()
                .ToTable(t =>
                    {
                        t.HasCheckConstraint(
                            "CHK_AELastModifiedDate",
                            "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate"
                        );

                        t.HasCheckConstraint(
                            "CHK_AECreatedDate",
                            "CreatedDate <= LastModifiedDate"
                        );
                    }
                )
                .HasOne<Employee>()
                .WithMany(e => e.AttendanceEntries)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
            
            //-----------------------------------------------------------------------------------------------------------
            // Holiday
            //-----------------------------------------------------------------------------------------------------------
            builder.Entity<Holiday>()
                .ToTable(t =>
                    {
                        t.HasCheckConstraint(
                            "CHK_HLastModifiedDate",
                            "LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate"
                        );

                        t.HasCheckConstraint(
                            "CHK_HCreatedDate",
                            "CreatedDate <= LastModifiedDate"
                        );
                    }
                );
            
        }
    }
}