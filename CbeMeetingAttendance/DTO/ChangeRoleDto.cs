namespace CbeMeetingAttendance.DTOs
{
    public class ChangeRoleDto
    {
        public Guid UserId { get; set; }

        public string NewRole { get; set; } = string.Empty;
    }
}