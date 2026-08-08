using Microsoft.AspNetCore.Identity;

namespace CbeMeetingAttendance.Models
{
    public class User : IdentityUser<Guid>
    {
        public Guid? EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;
    }
}