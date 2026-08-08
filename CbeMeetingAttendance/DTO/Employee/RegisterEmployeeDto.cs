using System.ComponentModel.DataAnnotations;

namespace CbeMeetingAttendance.DTOs.Employee
{
    public class RegisterEmployeeDto
    {
        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string Division { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }
    }
}