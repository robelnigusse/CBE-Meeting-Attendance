using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CbeMeetingAttendance.DTOs.Profile
{
    public class UploadProfileImageDto
    {
        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}