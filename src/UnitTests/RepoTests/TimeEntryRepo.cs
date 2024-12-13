using Core.Interfaces;
using Core.Models;
using Infrastructure.Repository;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.RepoTests
{
    public class TimeEntryRepoTests
    {
        private readonly Mock<IDbConnectionFactory> _mockConnectionFactory;
        private readonly Mock<IDapperWrapper> _mockDapperWrapper;
        private readonly TimeEntryRepo _timeEntryRepo;

        public TimeEntryRepoTests()
        {
            _mockConnectionFactory = new Mock<IDbConnectionFactory>();
            _mockDapperWrapper = new Mock<IDapperWrapper>();

            var mockDbConnection = new Mock<IDbConnection>();
            _mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(mockDbConnection.Object);

            _timeEntryRepo = new TimeEntryRepo(_mockConnectionFactory.Object, _mockDapperWrapper.Object);
        }

        [Fact]
        public async Task GetTimeEntriesAsync_ShouldReturnTimeEntries_ForGivenEmployeeId()
        {
            var employeeId = 123;
            var expectedEntries = new List<TimeEntry>
            {
                new TimeEntry { Id = 1, EmployeeId = employeeId, Date = DateTime.Now, HoursWorked = 8 },
                new TimeEntry { Id = 2, EmployeeId = employeeId, Date = DateTime.Now.AddDays(-1), HoursWorked = 7 }
            };

            _mockDapperWrapper
                .Setup(d => d.QueryAsync<TimeEntry>(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.Is<object>(p => (int)p.GetType().GetProperty("EmployeeId").GetValue(p) == employeeId)))
                .ReturnsAsync(expectedEntries);

            var result = await _timeEntryRepo.GetTimeEntriesAsync(employeeId);

            Assert.NotNull(result);
            Assert.Equal(expectedEntries.Count, result.Count());
        }

        [Fact]
        public async Task AddTimeEntryAsync_ShouldInsertTimeEntry()
        {
            var timeEntry = new TimeEntry
            {
                EmployeeId = 123,
                Date = DateTime.Now,
                HoursWorked = 8
            };

            _mockDapperWrapper
                .Setup(d => d.ExecuteAsync(
                    It.IsAny<IDbConnection>(),
                    It.IsAny<string>(),
                    It.Is<object>(p => (int)p.GetType().GetProperty("EmployeeId").GetValue(p) == timeEntry.EmployeeId)))
                .ReturnsAsync(1);

            await _timeEntryRepo.AddTimeEntryAsync(timeEntry);

            _mockDapperWrapper.Verify(d => d.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.Is<object>(p => (int)p.GetType().GetProperty("EmployeeId").GetValue(p) == timeEntry.EmployeeId)),
                Times.Once);
        }
    }
}