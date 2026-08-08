using Microsoft.AspNetCore.Mvc;
using CbeMeetingAttendance.DTOs.Attendance;
using CbeMeetingAttendance.Services;

namespace CbeMeetingAttendance.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _attendanceService;

        public AttendanceController(AttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // GET: api/attendance/check?employeeId=EMP001
        [HttpGet("check")]
        public async Task<IActionResult> CheckAttendance([FromQuery] string employeeId)
        {
            var result =
                await _attendanceService.CheckAttendanceAsync(employeeId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // POST: api/attendance
        [HttpPost]
        public async Task<IActionResult> TakeAttendance(AttendanceRequestDto dto)
        {
            var result =
                await _attendanceService.TakeAttendanceAsync(dto.EmployeeId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}