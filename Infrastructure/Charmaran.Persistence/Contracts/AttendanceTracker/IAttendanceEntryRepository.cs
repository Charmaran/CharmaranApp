using System.Collections.Generic;
using System.Threading.Tasks;
using Charmaran.Domain.Entities.AttendanceTracker;

namespace Charmaran.Persistence.Contracts.AttendanceTracker
{
    public interface IAttendanceEntryRepository : IAsyncRepository<AttendanceEntry>
    {
        Task<IEnumerable<AttendanceEntry>?> ListAsync(int employeeId);
    }
}