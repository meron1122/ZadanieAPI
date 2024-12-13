using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [Route("api/employees/{employeeId}/[controller]")]
    [ApiController]
    public class TimeEntriesController : ControllerBase
    {
        private readonly ITimeEntryRepo _timeEntryRepository;
        private readonly IEmployeeRepo _employeeRepository;

        public TimeEntriesController(ITimeEntryRepo timeEntryRepository, IEmployeeRepo employeeRepository)
        {
            _timeEntryRepository = timeEntryRepository;
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeEntries(int employeeId)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            var timeEntries = await _timeEntryRepository.GetTimeEntriesAsync(employeeId);
            return Ok(timeEntries);
        }

        [HttpGet("{entryId}")]
        public async Task<IActionResult> GetTimeEntry(int employeeId, int entryId)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            var timeEntry = await _timeEntryRepository.GetTimeEntryByIdAsync(employeeId, entryId);
            if (timeEntry == null)
            {
                return NotFound("Time entry not found.");
            }

            return Ok(timeEntry);
        }

        [HttpPost]
        public async Task<IActionResult> AddTimeEntry(int employeeId, [FromBody] TimeEntry timeEntry)
        {
            if (timeEntry == null)
                return BadRequest(new { error = "TimeEntry data cannot be null" });

            try
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
                if (employee == null)
                    return NotFound(new { error = "Employee not found." });

                if (timeEntry.Date == default)
                    return BadRequest(new { error = "Invalid date." });

                if (timeEntry.HoursWorked < 1 || timeEntry.HoursWorked > 24)
                    return BadRequest(new { error = "Hours worked must be between 1 and 24." });

                var existingEntries = await _timeEntryRepository.GetTimeEntriesAsync(employeeId);
                if (existingEntries.Any(te => te.Date.Date == timeEntry.Date.Date))
                    return Conflict(new { error = "Time entry for the specified date already exists." });

                timeEntry.EmployeeId = employeeId;
                await _timeEntryRepository.AddTimeEntryAsync(timeEntry);

                return CreatedAtAction(nameof(GetTimeEntry), new { employeeId = employeeId, entryId = timeEntry.Id }, timeEntry);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPut("{entryId}")]
        public async Task<IActionResult> UpdateTimeEntry(int employeeId, int entryId, [FromBody] TimeEntry timeEntry)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            var existingEntry = await _timeEntryRepository.GetTimeEntryByIdAsync(employeeId, entryId);
            if (existingEntry == null)
            {
                return NotFound("Time entry not found.");
            }

            if (timeEntry.Date == default)
            {
                return BadRequest("Invalid date.");
            }

            if (timeEntry.HoursWorked < 1 || timeEntry.HoursWorked > 24)
            {
                return BadRequest("Hours worked must be between 1 and 24.");
            }

            existingEntry.Date = timeEntry.Date;
            existingEntry.HoursWorked = timeEntry.HoursWorked;

            await _timeEntryRepository.UpdateTimeEntryAsync(existingEntry);

            return NoContent();
        }

        [HttpDelete("{entryId}")]
        public async Task<IActionResult> DeleteTimeEntry(int employeeId, int entryId)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            var timeEntry = await _timeEntryRepository.GetTimeEntryByIdAsync(employeeId, entryId);
            if (timeEntry == null)
            {
                return NotFound("Time entry not found.");
            }

            await _timeEntryRepository.DeleteTimeEntryAsync(employeeId, entryId);

            return NoContent();
        }
    }
}