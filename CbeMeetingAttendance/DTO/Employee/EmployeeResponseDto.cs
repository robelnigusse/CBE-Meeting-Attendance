namespace CbeMeetingAttendance.DTOs.Employee
{
    public class EmployeeResponseDto
    {
        public Guid Id { get; set; }

        public string EmployeeId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Division { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}