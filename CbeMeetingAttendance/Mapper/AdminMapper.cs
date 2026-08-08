using CbeMeetingAttendance.DTOs.Admin;
using CbeMeetingAttendance.Models;

namespace CbeMeetingAttendance.Mappers
{
    public static class AdminMapper
    {
        public static AttendanceSummaryDto ToAttendanceSummaryDto(Attendance attendance)
        {
            return new AttendanceSummaryDto
            {
                EmployeeId = attendance.Employee!.EmployeeId,
                FullName = attendance.Employee.FullName,
                Department = attendance.Employee.Department,
                Division = attendance.Employee.Division,
                Session = attendance.Session.ToString(),
                MeetingDate = attendance.MeetingDate,
                AttendanceTime = attendance.AttendanceTime
            };
        }

        public static List<AttendanceSummaryDto> ToAttendanceSummaryDtoList(
            List<Attendance> attendances)
        {
            return attendances
                .Select(ToAttendanceSummaryDto)
                .ToList();
        }
    }
}