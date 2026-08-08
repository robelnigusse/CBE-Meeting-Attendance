using System.ComponentModel.DataAnnotations;

namespace CbeMeetingAttendance.DTOs.Attendance
{
    public class AttendanceRequestDto
    {
        [Required]
        public string EmployeeId { get; set; } = string.Empty;
    }
}