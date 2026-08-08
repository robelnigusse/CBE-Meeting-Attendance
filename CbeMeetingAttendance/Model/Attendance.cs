using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CbeMeetingAttendance.Enums;

namespace CbeMeetingAttendance.Models
{
    public class Attendance
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

        public DateOnly MeetingDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public Session Session { get; set; }

        public DateTime AttendanceTime { get; set; } = DateTime.UtcNow;
    }
}