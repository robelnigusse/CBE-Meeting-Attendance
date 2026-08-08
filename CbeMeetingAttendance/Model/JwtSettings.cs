namespace CbeMeetingAttendance.Models
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;

        public string[] Audiences { get; set; } = [];

        public int ExpiryMinutes { get; set; }
    }
}