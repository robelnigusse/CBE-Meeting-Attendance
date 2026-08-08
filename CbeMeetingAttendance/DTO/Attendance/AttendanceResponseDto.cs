namespace CbeMeetingAttendance.DTOs.Attendance
{
    public class AttendanceResponseDto
    {
        public string EmployeeId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public DateOnly MeetingDate { get; set; }

        public string Session { get; set; } = string.Empty;

        public DateTime AttendanceTime { get; set; }
    }
}