using System.Collections.Generic;
using System.Threading.Tasks;
using Charmaran.Domain.Entities.AttendanceTracker;

namespace Charmaran.Persistence.Contracts.AttendanceTracker
{
    public interface IEmployeeRepository : IAsyncRepository<Employee>
    {
        Task<IEnumerable<Employee>?> ListAllAsync();
    }
}