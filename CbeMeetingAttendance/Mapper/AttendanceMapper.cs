using CbeMeetingAttendance.DTOs.Attendance;
using CbeMeetingAttendance.Models;

namespace CbeMeetingAttendance.Mappers
{
    public static class AttendanceMapper
    {
        public static AttendanceResponseDto ToAttendanceResponseDto(Attendance attendance)
        {
            return new AttendanceResponseDto
            {
                EmployeeId = attendance.Employee!.EmployeeId,
                FullName = attendance.Employee.FullName,
                MeetingDate = attendance.MeetingDate,
                Session = attendance.Session.ToString(),
                AttendanceTime = attendance.AttendanceTime
            };
        }

        public static AttendanceCheckResponseDto ToAttendanceCheckResponseDto(
            Attendance attendance,
            bool attended)
        {
            return new AttendanceCheckResponseDto
            {
                Attended = attended,
                EmployeeId = attendance.Employee!.EmployeeId,
                FullName = attendance.Employee.FullName,
                Session = attendance.Session.ToString(),
                MeetingDate = attendance.MeetingDate
            };
        }
    }
}