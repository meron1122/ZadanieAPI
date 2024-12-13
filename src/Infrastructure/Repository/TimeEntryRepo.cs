using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Repository
{
    public class TimeEntryRepo : ITimeEntryRepo
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IDapperWrapper _dapperWrapper;

        public TimeEntryRepo(IDbConnectionFactory connectionFactory, IDapperWrapper dapperWrapper)
        {
            _connectionFactory = connectionFactory;
            _dapperWrapper = dapperWrapper;
        }


        public async Task<IEnumerable<TimeEntry>> GetTimeEntriesAsync(int employeeId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await _dapperWrapper.QueryAsync<TimeEntry>(
                connection,
                "SELECT * FROM TimeEntries WHERE EmployeeId = @EmployeeId",
                new { EmployeeId = employeeId });
        }

        public async Task<TimeEntry> GetTimeEntryByIdAsync(int employeeId, int entryId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var timeEntry = await _dapperWrapper.QuerySingleOrDefaultAsync<TimeEntry>(
                connection,
                "SELECT * FROM TimeEntries WHERE EmployeeId = @EmployeeId AND Id = @Id",
                new { EmployeeId = employeeId, Id = entryId });
            if (timeEntry == null)
                throw new KeyNotFoundException($"Time entry with {entryId} for employee {employeeId} was not found");
            else
                return timeEntry;
        }

        public async Task AddTimeEntryAsync(TimeEntry timeEntry)
        {
            using var connection = _connectionFactory.CreateConnection();
            await _dapperWrapper.ExecuteAsync(
                connection,
                "INSERT INTO TimeEntries (EmployeeId, Date, HoursWorked) VALUES (@EmployeeId, @Date, @HoursWorked)",
                timeEntry);
        }

        public async Task UpdateTimeEntryAsync(TimeEntry timeEntry)
        {
            using var connection = _connectionFactory.CreateConnection();
            await _dapperWrapper.
                ExecuteAsync(connection,"UPDATE TimeEntries SET Date = @Date, HoursWorked = @HoursWorked WHERE Id = @Id AND EmployeeId = @EmployeeId", timeEntry);
        }

        public async Task DeleteTimeEntryAsync(int employeeId, int entryId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await _dapperWrapper.ExecuteAsync(
                connection,
                "DELETE FROM TimeEntries WHERE EmployeeId = @EmployeeId AND Id = @Id",
                new { EmployeeId = employeeId, Id = entryId });
        }
    }
}
