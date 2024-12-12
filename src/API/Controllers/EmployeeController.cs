using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepo _repository;

        public EmployeeController(IEmployeeRepo repo)
        {
            _repository = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _repository.GetEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _repository.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            if(employee == null)
                return BadRequest(new { error = "Employee data cannot be null." });
            try
            {
                await _repository.AddEmployeeAsync(employee);
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error ="An unexpected error occured.", details=ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            await _repository.UpdateEmployeeAsync(employee);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _repository.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound(new { message = $"Employee with ID {id} was not found" });

            await _repository.DeleteEmployeeAsync(id);

            return Ok(new { message = $"Employee with ID {id} was successfully deleted.", employee });
        }
    }
}
