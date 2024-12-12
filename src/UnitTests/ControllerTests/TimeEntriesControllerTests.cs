using API.Controllers;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class TimeEntriesControllerTests
    {
        private readonly Mock<ITimeEntryRepo> _timeRepoMock;
        private readonly Mock<IEmployeeRepo> _employeeRepoMock;
        private readonly TimeEntriesController _controller;

        public TimeEntriesControllerTests()
        {
            _timeRepoMock = new Mock<ITimeEntryRepo>();
            _employeeRepoMock = new Mock<IEmployeeRepo>();
            _controller = new TimeEntriesController(_timeRepoMock.Object, _employeeRepoMock.Object);
        }

        [Fact]
        public async Task GetTimeEntries_ShouldReturnOkResult_WithTimeEntries()
        {
            var timeEntries = new List<TimeEntry>
            {
                new TimeEntry { Id = 1, EmployeeId = 1, Date = DateTime.Now, HoursWorked = 8 }
            };

            _employeeRepoMock.Setup(repo => repo.GetEmployeeByIdAsync(1)).ReturnsAsync(new Employee());
            _timeRepoMock.Setup(repo => repo.GetTimeEntriesAsync(1)).ReturnsAsync(timeEntries);

            var result = await _controller.GetTimeEntries(1);

            var isOk = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(timeEntries, isOk.Value);
        }

        [Fact]
        public async Task AddTimeEntry_ShouldReturnError_WhenDateAlreadyExist()
        {
            var timeEntry = new TimeEntry { EmployeeId = 1, Date = DateTime.Now, HoursWorked = 8 };
            _employeeRepoMock.Setup(repo => repo.GetEmployeeByIdAsync(1)).ReturnsAsync(new Employee());
            _timeRepoMock.Setup(repo => repo.GetTimeEntriesAsync(1)).ReturnsAsync(new List<TimeEntry> { timeEntry });

            var result = await _controller.AddTimeEntry(1, timeEntry);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(conflictResult.Value);
            var errorObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
            Assert.Equal("Time entry for the specified date already exists.", (string)errorObject.error);
        }

        [Fact]
        public async Task UpdateTimeEntry_ShouldReturnNoContent_WhenEntryIsUpdated()
        {
            var existingEntry = new TimeEntry { Id = 1, EmployeeId = 1, Date = DateTime.Now, HoursWorked = 8 };
            _employeeRepoMock.Setup(repo => repo.GetEmployeeByIdAsync(1)).ReturnsAsync(new Employee());
            _timeRepoMock.Setup(repo => repo.GetTimeEntryByIdAsync(1, 1)).ReturnsAsync(existingEntry);

            var result = await _controller.UpdateTimeEntry(1, 1, existingEntry);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddTimeEntry_ShouldReturnError_WhenDateMoreThan24()
        {
            var timeEntry = new TimeEntry { Id = 1, EmployeeId = 1, Date = DateTime.Now, HoursWorked = 25 };
            _employeeRepoMock.Setup(repo => repo.GetEmployeeByIdAsync(1)).ReturnsAsync(new Employee());
            _timeRepoMock.Setup(repo => repo.GetTimeEntryByIdAsync(1, 1)).ReturnsAsync(timeEntry);

            var result = await _controller.AddTimeEntry(1, timeEntry);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(badRequestResult.Value);
            var errorObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
            Assert.Equal("Hours worked must be between 1 and 24.", (string)errorObject.error);
        }

    }
}
