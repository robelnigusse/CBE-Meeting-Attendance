namespace CbeMeetingAttendance.DTOs
{
    public class CurrentUserDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public Guid? EmployeeId { get; set; }

        public string? EmployeeName { get; set; }

        public string? Division { get; set; }

        public string? Department { get; set; }

        public string? JobTitle { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }
}