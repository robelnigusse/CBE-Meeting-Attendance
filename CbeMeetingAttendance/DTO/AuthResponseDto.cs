namespace CbeMeetingAttendance.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }

        public CurrentUserDto User { get; set; } = null!;
    }
}