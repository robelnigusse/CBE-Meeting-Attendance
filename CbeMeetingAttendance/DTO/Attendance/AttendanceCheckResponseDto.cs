namespace CbeMeetingAttendance.DTOs.Attendance
{
    public class AttendanceCheckResponseDto
    {
        public bool Attended { get; set; }

        public string EmployeeId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Session { get; set; } = string.Empty;

        public DateOnly MeetingDate { get; set; }
    }
}