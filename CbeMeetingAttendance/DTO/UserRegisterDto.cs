namespace CbeMeetingAttendance.DTOs
{
    public class UserRegisterDto
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string? EmployeeId { get; set; }
    }
}