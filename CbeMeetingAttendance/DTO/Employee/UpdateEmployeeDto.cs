using System.ComponentModel.DataAnnotations;

namespace CbeMeetingAttendance.DTOs.Employee
{
    public class UpdateEmployeeDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public string Division { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
    }
}