using Core.Interfaces;
using Core.Models;
using System.Text.RegularExpressions;

namespace Infrastructure.Repository
{
    public class EmployeeRepo : IEmployeeRepo
    {

        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IDapperWrapper _dapperWrapper;

        public EmployeeRepo(IDbConnectionFactory connectionFactory, IDapperWrapper dapperWrapper)
        {
            _connectionFactory = connectionFactory;
            _dapperWrapper = dapperWrapper;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await _dapperWrapper.QueryAsync<Employee>(connection,"SELECT * FROM employees");
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await _dapperWrapper.QuerySingleOrDefaultAsync<Employee>(connection,"SELECT * FROM Employees WHERE Id = @Id",
                                                                        new { Id = id });
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            ValidateField(employee.FirstName, nameof(employee.FirstName), true);
            ValidateField(employee.LastName, nameof(employee.LastName), true);
            ValidateField(employee.Email, nameof(employee.Email), true, isEmail: true);

            employee = NormalizeEmployee(employee);

            using var connection = _connectionFactory.CreateConnection();
            await _dapperWrapper.ExecuteAsync(connection,"INSERT INTO Employees (FirstName, LastName, Email) VALUES (@FirstName, @LastName, @Email)", employee);

            return employee;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            ValidateField(employee.FirstName, nameof(employee.FirstName), false);
            ValidateField(employee.LastName, nameof(employee.LastName), false);
            ValidateField(employee.Email, nameof(employee.Email), false, isEmail: true);

            using var connection = _connectionFactory.CreateConnection();
            await _dapperWrapper.ExecuteAsync(connection,"UPDATE Employees SET FirstName = @FirstName, LastName = @LastName, Email = @Email WHERE Id = @Id", employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await _dapperWrapper.ExecuteAsync(connection,"DELETE FROM Employees WHERE Id = @Id", new { Id = id });
        }

        private void ValidateField(string? value, string fieldName, bool isRequired, bool isEmail = false)
        {
            if (isRequired && string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} cannot be null or empty.");

            if (isEmail && !string.IsNullOrWhiteSpace(value) &&
                !Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
                throw new ArgumentException($"{fieldName} must be in a valid format. Provided value: '{value}'");
        }

        private Employee NormalizeEmployee(Employee employee)
        {
            if (!string.IsNullOrWhiteSpace(employee.FirstName))
                employee.FirstName = char.ToUpper(employee.FirstName[0]) + employee.FirstName.Substring(1).ToLower();

            if (!string.IsNullOrWhiteSpace(employee.LastName))
                employee.LastName = char.ToUpper(employee.LastName[0]) + employee.LastName.Substring(1).ToLower();

            return employee;
        }
    }
}

