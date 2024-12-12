using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.IntegrationTests
{
    public class IntegrationTests
    {
        private readonly HttpClient _client;

        public IntegrationTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:10310")
            };
        }

        [Fact]
        public async Task AddAndDeleteEmployee_ShouldSucceed()
        {
            var employee = new
            {
                FirstName = "Marek",
                LastName = "Szymanski",
                Email = "Marek.Szymanski@gmail.com"
            };
            var employeeContent = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");

            var credentials = Encoding.ASCII.GetBytes("admin:password123");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));

            var addEmployeeResponse = await _client.PostAsync("/api/Employee", employeeContent);
            Assert.Equal(HttpStatusCode.Created, addEmployeeResponse.StatusCode);

            var getEmployeesResponse = await _client.GetAsync("/api/Employee");
            Assert.Equal(HttpStatusCode.OK, getEmployeesResponse.StatusCode);

            var employeesContent = await getEmployeesResponse.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<dynamic>>(employeesContent);

            var createdEmployee = employees.FirstOrDefault(e => e.email == employee.Email);
            Assert.NotNull(createdEmployee);

            var employeeId = (int)createdEmployee.id;
            Assert.True(employeeId > 0, "The employee ID should be greater than 0.");

            var deleteResponse = await _client.DeleteAsync($"/api/Employee/{employeeId}");

            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task AddAndDeleteTimeEntry_ShouldSucceed()
        {
            var employee = new
            {
                FirstName = "Test",
                LastName = "Developer",
                Email = "Marek@test.pl"
            };
            var employeeContent = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");

            var credentials = Encoding.ASCII.GetBytes("admin:password123");
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));

            var addEmployeeResponse = await _client.PostAsync("/api/Employee", employeeContent);
            Assert.Equal(HttpStatusCode.Created, addEmployeeResponse.StatusCode);

            var getEmployeesResponse = await _client.GetAsync("/api/Employee");
            Assert.Equal(HttpStatusCode.OK, getEmployeesResponse.StatusCode);

            var employeesContent = await getEmployeesResponse.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<dynamic>>(employeesContent);

            var createdEmployee = employees.FirstOrDefault(e => e.email == employee.Email);
            Assert.NotNull(createdEmployee);

            var employeeId = (int)createdEmployee.id;

            var timeEntry = new
            {
                Date = "2024-12-12T00:00:00",
                HoursWorked = 8
            };

            var timeEntryContent = new StringContent(JsonConvert.SerializeObject(timeEntry), Encoding.UTF8, "application/json");

            var addTimeEntryResponse = await _client.PostAsync($"/api/employees/{employeeId}/TimeEntries", timeEntryContent);
            Assert.Equal(HttpStatusCode.Created, addTimeEntryResponse.StatusCode);

            var getTimeEntriesResponse = await _client.GetAsync($"/api/employees/{employeeId}/TimeEntries");
            Assert.Equal(HttpStatusCode.OK, getTimeEntriesResponse.StatusCode);

            var timeEntriesContent = await getTimeEntriesResponse.Content.ReadAsStringAsync();
            var timeEntries = JsonConvert.DeserializeObject<List<dynamic>>(timeEntriesContent);

            var createdTimeEntry = timeEntries.FirstOrDefault(te => te.date == timeEntry.Date);
            Assert.NotNull(createdTimeEntry);

            var timeEntryId = (int)createdTimeEntry.id;
            Assert.True(timeEntryId > 0, "The time entry ID should be greater than 0.");

            var deleteTimeEntryResponse = await _client.DeleteAsync($"/api/employees/{employeeId}/TimeEntries/{timeEntryId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteTimeEntryResponse.StatusCode);

            var deleteEmployeeResponse = await _client.DeleteAsync($"/api/Employee/{employeeId}");
            Assert.Equal(HttpStatusCode.OK, deleteEmployeeResponse.StatusCode);
        }
    }
}
