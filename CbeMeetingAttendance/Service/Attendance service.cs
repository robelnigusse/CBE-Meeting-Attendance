using CbeMeetingAttendance.DTOs;
using CbeMeetingAttendance.DTOs.Attendance;
using CbeMeetingAttendance.Enums;
using CbeMeetingAttendance.Mappers;
using CbeMeetingAttendance.Models;
using CbeMeetingAttendance.Repositories;
using Microsoft.Extensions.Configuration;

namespace CbeMeetingAttendance.Services
{
    public class AttendanceService
    {
        private readonly AttendanceRepository _attendanceRepository;
        private readonly EmployeeRepository _employeeRepository;
        private readonly IConfiguration _configuration;

        public AttendanceService(
            AttendanceRepository attendanceRepository,
            EmployeeRepository employeeRepository,
            IConfiguration configuration)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _configuration = configuration;
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
        public async Task<ApiResponse<AttendanceResponseDto>> TakeAttendanceAsync(AttendanceRequestDto dto)
        {
            var employee = await _employeeRepository.GetByEmployeeIdAsync(dto.EmployeeId);

            if (employee == null)
            {
                return new ApiResponse<AttendanceResponseDto>
                {
                    Success = false,
                    Message = "Employee not found."
                };
            }

            if (!dto.Latitude.HasValue || !dto.Longitude.HasValue)
            {
                return new ApiResponse<AttendanceResponseDto>
                {
                    Success = false,
                    Message = "Location coordinates are required."
                };
            }

            double configLat = _configuration.GetValue<double>("AttendanceLocation:Latitude");
            double configLon = _configuration.GetValue<double>("AttendanceLocation:Longitude");
            double configRadius = _configuration.GetValue<double>("AttendanceLocation:RadiusMeters");
            Console.WriteLine($"configLat: {configLat}, configLon: {configLon}, configRadius: {configRadius}");
            double distance = CalculateDistance(dto.Latitude.Value, dto.Longitude.Value, configLat, configLon);
            if (distance > configRadius)
            {
                return new ApiResponse<AttendanceResponseDto>
                {
                    Success = false,
                    Message = "You are outside the allowed attendance location."
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

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; // metres
            var p1 = lat1 * Math.PI / 180; // φ, λ in radians
            var p2 = lat2 * Math.PI / 180;
            var dp = (lat2 - lat1) * Math.PI / 180;
            var dl = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(dp / 2) * Math.Sin(dp / 2) +
                    Math.Cos(p1) * Math.Cos(p2) *
                    Math.Sin(dl / 2) * Math.Sin(dl / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // in metres
        }
    }
}