namespace CbeMeetingAttendance.DTOs.Profile
{
    public class ProfileResponseDto
    {
        public Guid EmployeeId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }
    }
}