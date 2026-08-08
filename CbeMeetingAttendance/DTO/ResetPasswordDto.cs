namespace CbeMeetingAttendance.DTOs
{
    public class ResetPasswordDto
    {
        public Guid UserId { get; set; }

        public string Token { get; set; }
            = string.Empty;


        public string NewPassword { get; set; }
            = string.Empty;
    }
}