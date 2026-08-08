using CbeMeetingAttendance.DTOs.Profile;
using CbeMeetingAttendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace CbeMeetingAttendance.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }


        // Upload or update profile image
        [HttpPost("upload")]
        public async Task<IActionResult> UploadProfileImage(
            [FromForm] UploadProfileImageDto dto)
        {
            var result =
                await _profileService.UploadProfileImageAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }



        // Get profile image from database bytes
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetProfileImage(
            string employeeId)
        {
            var image =
                await _profileService.GetProfileImageAsync(employeeId);


            if (image == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Profile image not found"
                });
            }


            return File(
                image,
                "image/jpeg");
        }



        // Delete profile image
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteProfileImage(
            string employeeId)
        {
            var result =
                await _profileService.DeleteProfileImageAsync(employeeId);


            if (!result.Success)
            {
                return BadRequest(result);
            }


            return Ok(result);
        }
    }
}