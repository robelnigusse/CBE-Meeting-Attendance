namespace CbeMeetingAttendance.DTOs.Admin
{
    public class AttendanceSummaryDto
    {
        public string EmployeeId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Division { get; set; } = string.Empty;

        public string Session { get; set; } = string.Empty;

        public DateOnly MeetingDate { get; set; }

        public DateTime AttendanceTime { get; set; }
    }
}