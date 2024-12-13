using Core.Interfaces;
using Core.Models;
using Infrastructure.Repository;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.RepoTests
{
    public class EmployeeRepoTests
    {
        private readonly Mock<IDbConnectionFactory> _mockConnectionFactory;
        private readonly Mock<IDapperWrapper> _mockDapperWrapper;
        private readonly EmployeeRepo _employeeRepo;

        public EmployeeRepoTests()
        {
            _mockConnectionFactory = new Mock<IDbConnectionFactory>();
            _mockDapperWrapper = new Mock<IDapperWrapper>();

            var mockDbConnection = new Mock<IDbConnection>();
            _mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(mockDbConnection.Object);

            _employeeRepo = new EmployeeRepo(_mockConnectionFactory.Object, _mockDapperWrapper.Object);
        }

        [Fact]
        public async Task AddEmployeeAsync_ShouldThrowArgumentException_WhenFirstNameIsEmpty()
        { 
            var employee = new Employee { FirstName = "", LastName = "Szymanski", Email = "test@test.com" };

            await Assert.ThrowsAsync<ArgumentException>(() => _employeeRepo.AddEmployeeAsync(employee));
        }

        [Fact]
        public async Task AddEmployeeAsync_ShouldThrowArgumentException_WhenEmailIsInvalid()
        {
            var employee = new Employee { FirstName = "Marek", LastName = "Szymanski", Email = "test" };

            await Assert.ThrowsAsync<ArgumentException>(() => _employeeRepo.AddEmployeeAsync(employee));
        }

        [Fact]
        public async Task AddEmployeeAsync_ShouldNormalizeEmployeeNames()
        {
            var employee = new Employee
            {
                FirstName = "maRek",
                LastName = "szyManski",
                Email = "test@test.com"
            };

            _mockDapperWrapper
                .Setup(d => d.ExecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(1);

            var result = await _employeeRepo.AddEmployeeAsync(employee);

            Assert.Equal("Marek", result.FirstName);
            Assert.Equal("Szymanski", result.LastName);
        }
   
    }
}
