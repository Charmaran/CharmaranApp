using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charmaran.Domain.Entities.AttendanceTracker;
using Charmaran.Persistence.Contracts.AttendanceTracker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Charmaran.Persistence.Repositories.AttendanceTracker
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ILogger<EmployeeRepository> _logger;
        private readonly CharmaranDbContext _dbContext;

        public EmployeeRepository(ILogger<EmployeeRepository> logger, CharmaranDbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }
        
        public async Task<Employee?> GetByIdAsync(int id)
        {
            Employee? employee = null;
            
            try
            {
                employee = await this._dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"SQL Error when fetching Employee row for {id}");
            }

            return employee;
        }

        public async Task<int> AddAsync(Employee entity)
        {
            await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await this._dbContext.Employees.AddAsync(entity);
                    await this._dbContext.SaveChangesAsync();
                    
                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, "SQL error creating Employee");
                    await transaction.RollbackAsync();
                }
            }

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Employee entity)
        {
            int rowsAffected = 0;
            
            await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    Employee? existingEmployee = await this.GetByIdAsync(entity.Id);
                    
                    if (existingEmployee == null)
                    {
                        return false;
                    }
                    
                    existingEmployee.Name = entity.Name;
                    existingEmployee.IsDeleted = entity.IsDeleted;
                    existingEmployee.LastModifiedBy = entity.LastModifiedBy;
                    existingEmployee.LastModifiedDate = entity.LastModifiedDate;
                    
                    this._dbContext.Employees.Update(entity);
                    rowsAffected = await this._dbContext.SaveChangesAsync();
                    
                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, $"SQL error updating Employee {entity.Id}");
                    await transaction.RollbackAsync();
                }
            }
            
            return rowsAffected == 1;
        }

        public async Task<bool> DeleteAsync(Employee entity)
        {
            int rowsAffected = 0;
            
            await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    this._dbContext.Employees.Remove(entity);
                    rowsAffected = await this._dbContext.SaveChangesAsync();
                    
                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, $"SQL error deleting Employee {entity.Id}");
                    await transaction.RollbackAsync();
                }
            }
            
            return rowsAffected == 1;
        }

        public async Task<IEnumerable<Employee>?> ListAllAsync()
        {
            IEnumerable<Employee> employees;
            
            try
            {
                employees = await this._dbContext.Employees.Where(e => e.IsDeleted == false).ToArrayAsync();
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "SQL error fetching all Employees");
                return null;
            }

            return employees;
        }

        public async Task<bool> IsEmployeeNameUnique(string name)
        {
            try
            {
                return await this._dbContext.Employees.FirstOrDefaultAsync(e => e.Name.Equals(name)) == null;
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"SQL error checking if Employee name {name} is unique");
            }

            return true;
        }

        public async Task<bool> EmployeeExists(int id)
        {
            Employee? employee= await this.GetByIdAsync(id);
            return employee != null;
        }
    }
}