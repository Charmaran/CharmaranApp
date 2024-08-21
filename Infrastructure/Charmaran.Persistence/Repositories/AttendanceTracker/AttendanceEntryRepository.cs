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
    public class AttendanceEntryRepository : IAttendanceEntryRepository
    {
        private readonly ILogger<AttendanceEntryRepository> _logger;
        private readonly CharmaranDbContext _dbContext;

        public AttendanceEntryRepository(ILogger<AttendanceEntryRepository> logger, CharmaranDbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }
        
        public async Task<AttendanceEntry?> GetByIdAsync(int id)
        {
            AttendanceEntry? attendanceEntry = null;
            
            try
            {
                attendanceEntry = await this._dbContext.AttendanceEntries.FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"SQL Error when fetching AttendanceEntry row for {id}");
            }

            return attendanceEntry;
        }

        public async Task<int> AddAsync(AttendanceEntry entity)
        {
            await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try 
                {
                    await this._dbContext.AttendanceEntries.AddAsync(entity);
                    await this._dbContext.SaveChangesAsync();
                    
                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, "SQL error creating AttendanceEntry");
                    await transaction.RollbackAsync();
                    return -1;
                }
            }
            
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(AttendanceEntry entity)
        {
            int rowsAffected = 0;
            
            await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    AttendanceEntry? existingEntry = await this.GetByIdAsync(entity.Id);

                    if (existingEntry == null)
                    {
                        return false;
                    }
                    
                    existingEntry.Amount = entity.Amount;
                    existingEntry.Category = entity.Category;
                    existingEntry.InputDate = entity.InputDate;
                    existingEntry.Notes = entity.Notes;
                    existingEntry.LastModifiedBy = entity.LastModifiedBy;
                    existingEntry.LastModifiedDate = entity.LastModifiedDate;

                    rowsAffected = await this._dbContext.SaveChangesAsync();
                    
                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, $"SQL error updating AttendanceEntry {entity.Id}");
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return rowsAffected == 1;
        }

        public async Task<bool> DeleteAsync(AttendanceEntry entity)
        {
            int rowsAffected = 0;
            
            await using (IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    this._dbContext.AttendanceEntries.Remove(entity);
                    rowsAffected = await this._dbContext.SaveChangesAsync();
                    
                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, $"SQL error deleting AttendanceEntry {entity.Id}");
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return rowsAffected == 1;
        }

        public async Task<IEnumerable<AttendanceEntry>?> ListAsync(int employeeId)
        {
            IEnumerable<AttendanceEntry>? attendanceEntries;
            
            try
            {
                attendanceEntries = await this._dbContext.AttendanceEntries.Where(a => a.EmployeeId == employeeId).ToArrayAsync();
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"SQL error fetching AttendanceEntries for Employee {employeeId}");
                return null;
            }
            
            return attendanceEntries;
        }
    }
}