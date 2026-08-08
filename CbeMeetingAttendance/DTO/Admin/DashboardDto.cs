namespace CbeMeetingAttendance.DTOs.Admin
{
    public class DashboardDto
    {
        public int TotalEmployees { get; set; }

        public int TodayAttendance { get; set; }

        public int MorningAttendance { get; set; }

        public int AfternoonAttendance { get; set; }

        public List<AttendanceSummaryDto> TodayAttendees { get; set; } = new();
    }
}