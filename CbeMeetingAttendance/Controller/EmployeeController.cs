using Microsoft.AspNetCore.Mvc;
using CbeMeetingAttendance.DTOs.Employee;
using CbeMeetingAttendance.Services;

namespace CbeMeetingAttendance.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;

        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // POST: api/employees/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterEmployeeDto dto)
        {
            var result = await _employeeService.RegisterEmployeeAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/employees
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _employeeService.GetAllEmployeesAsync());
        }

        // GET: api/employees/id/{id:guid}
        [HttpGet("id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);

            if (!result.Success)
                return Ok(result);

            return Ok(result);
        }

        // GET: api/employees/{employeeId}
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> CheckEmployee(string employeeId)
        {
            var result =
                await _employeeService.GetEmployeeByEmployeeIdAsync(employeeId);

            if (result == null)
                return Ok(result);

            return Ok(result);
        }

        // PUT: api/employees/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateEmployeeDto dto)
        {
            var result = await _employeeService.UpdateEmployeeAsync(id, dto);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}