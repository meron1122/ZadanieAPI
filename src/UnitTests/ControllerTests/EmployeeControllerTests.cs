using API.Controllers;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeRepo> _repoMock;
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            _repoMock = new Mock<IEmployeeRepo>();
            _controller = new EmployeeController(_repoMock.Object);
        }

        [Fact]
        public async Task GetEmployees_ShouldReturnOkResult_WithEmployees()
        {         
            var employees = new List<Employee>
            {
            new Employee { Id = 1, FirstName = "Marek", LastName = "Szymanski", Email = "test@test.pl" }
            };
            _repoMock.Setup(repo => repo.GetEmployeesAsync()).ReturnsAsync(employees);

            var result = await _controller.GetEmployees();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(employees, okResult.Value);
        }

        [Fact]
        public async Task GetEmployee_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
        {
            _repoMock.Setup(repo => repo.GetEmployeeByIdAsync(It.IsAny<int>())).ReturnsAsync((Employee?)null);

            var result = await _controller.GetEmployee(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddEmployee_ShouldReturnCreatedAtAction_WithEmployee()
        {
            var employee = new Employee { Id = 1, FirstName = "Marek", LastName = "Szymanski", Email = "test@test.pl" };

            var result = await _controller.AddEmployee(employee);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(employee, createdResult.Value);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldReturnBadRequest_WhenIdsDoNotMatch()
        {
            var employee = new Employee { Id = 1, FirstName = "Marek", LastName = "Szymanski", Email = "test@test.pl" };

            var result = await _controller.UpdateEmployee(2, employee);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteEmployee_ShouldReturnNoContent_WhenEmployeeIsDeleted()
        {
            _repoMock.Setup(repo => repo.DeleteEmployeeAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            var result = await _controller.DeleteEmployee(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}