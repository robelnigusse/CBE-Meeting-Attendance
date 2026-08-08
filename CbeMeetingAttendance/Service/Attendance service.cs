using CbeMeetingAttendance.DTOs;
using CbeMeetingAttendance.DTOs.Attendance;
using CbeMeetingAttendance.Enums;
using CbeMeetingAttendance.Mappers;
using CbeMeetingAttendance.Models;
using CbeMeetingAttendance.Repositories;

namespace CbeMeetingAttendance.Services
{
    public class AttendanceService
    {
        private readonly AttendanceRepository _attendanceRepository;
        private readonly EmployeeRepository _employeeRepository;

        public AttendanceService(
            AttendanceRepository attendanceRepository,
            EmployeeRepository employeeRepository)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
        }

        // Check Attendance
        public async Task<ApiResponse<AttendanceCheckResponseDto>> CheckAttendanceAsync(string employeeId)
        {
            var employee = await _employeeRepository.GetByEmployeeIdAsync(employeeId);

            if (employee == null)
            {
                return new ApiResponse<AttendanceCheckResponseDto>
                {
                    Success = false,
                    Message = "Employee not found."
                };
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            Session session = GetCurrentSession();

            bool attended = await _attendanceRepository.AttendanceExistsAsync(
                employee.Id,
                today,
                session);

            return new ApiResponse<AttendanceCheckResponseDto>
            {
                Success = true,
                Message = attended ? "Attended" : "Not Attended",

                Data = new AttendanceCheckResponseDto
                {
                    EmployeeId = employee.EmployeeId,
                    FullName = employee.FullName,
                    MeetingDate = today,
                    Session = session.ToString(),
                    Attended = attended
                }
            };
        }

        // Take Attendance
        public async Task<ApiResponse<AttendanceResponseDto>> TakeAttendanceAsync(string employeeId)
        {
            var employee = await _employeeRepository.GetByEmployeeIdAsync(employeeId);

            if (employee == null)
            {
                return new ApiResponse<AttendanceResponseDto>
                {
                    Success = false,
                    Message = "Employee not found."
                };
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            Session session = GetCurrentSession();

            bool exists = await _attendanceRepository.AttendanceExistsAsync(
                employee.Id,
                today,
                session);

            if (exists)
            {
                return new ApiResponse<AttendanceResponseDto>
                {
                    Success = false,
                    Message = "Attendance already taken."
                };
            }

            var attendance = new Attendance
            {
                EmployeeId = employee.Id,
                MeetingDate = today,
                Session = session,
                AttendanceTime = DateTime.UtcNow
            };

            attendance = await _attendanceRepository.AddAttendanceAsync(attendance);

            attendance.Employee = employee;

            return new ApiResponse<AttendanceResponseDto>
            {
                Success = true,
                Message = "Attendance recorded successfully.",
                Data = AttendanceMapper.ToAttendanceResponseDto(attendance)
            };
        }

        // Determine Morning or Afternoon
        private Session GetCurrentSession()
        {
            return DateTime.Now.Hour < 12
                ? Session.Morning
                : Session.Afternoon;
        }
    }
}