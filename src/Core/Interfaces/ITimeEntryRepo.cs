
using Core.Models;

namespace Core.Interfaces
{
    public interface ITimeEntryRepo
    {
        Task<IEnumerable<TimeEntry>> GetTimeEntriesAsync(int employeeId);
        Task<TimeEntry> GetTimeEntryByIdAsync(int employeeId, int entryId);
        Task AddTimeEntryAsync(TimeEntry timeEntry);
        Task UpdateTimeEntryAsync(TimeEntry timeEntry);
        Task DeleteTimeEntryAsync(int employeeId, int entryId);
    }
}
