using CbeMeetingAttendance.DTOs;
using CbeMeetingAttendance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CbeMeetingAttendance.Data;

namespace CbeMeetingAttendance.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;


        public UserController(
            UserService userService)
        {
            _userService = userService;
        }


        // Register new user
        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> Register(
            UserRegisterDto dto)
        {
            var result =
                await _userService.RegisterUser(dto);

            return Ok(result);
        }


        [AllowAnonymous]
        [HttpGet("RSAPublicKey")]
        public IActionResult GetRSAPublicKey()
        {
            var publicKey = RSAHelper.GetPublicKey();

            return Ok(new { Key = publicKey });
        }

        // Login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginDto dto)
        {
            var result =
                await _userService.Login(dto);

            return Ok(result);
        }



        // Refresh token
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            RefreshTokenRequestDto dto)
        {
            var result =
                await _userService.Refresh(dto);

            return Ok(result);
        }



        // Logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            RefreshTokenRequestDto dto)
        {
            var result =
                await _userService.Logout(dto);

            return Ok(result);
        }



        // Get all users
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var result =
                await _userService.GetAllUsers();

            return Ok(result);
        }



        // Get user by id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(
            Guid id)
        {
            var result =
                await _userService.GetUserById(id);

            return Ok(result);
        }



        // Assign role
        [HttpPost("assign-role")]
        [Authorize]
        public async Task<IActionResult> AssignRole(
            AssignRoleDto dto)
        {
            var result =
                await _userService.AssignRole(
                    dto.UserId,
                    dto.Role,
                    User);


            return Ok(result);
        }



        // Change role
        [HttpPut("change-role")]
        [Authorize]
        public async Task<IActionResult> ChangeRole(
            ChangeRoleDto dto)
        {
            var result =
                await _userService.ChangeRole(
                    dto,
                    User);


            return Ok(result);
        }



        // Remove role
        [HttpDelete("remove-role")]
        [Authorize]
        public async Task<IActionResult> RemoveRole(
    Guid userId,
    string role)
        {
            var result =
                await _userService.RemoveRole(
                    userId,
                    role,
                    User);


            return Ok(result);
        }



        // Change password
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordDto dto)
        {
            var userId =
                Guid.Parse(
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier)!);


            var result =
                await _userService.ChangePassword(
                    userId,
                    dto);


            return Ok(result);
        }



        // Reset password
        [HttpPost("reset-password")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordDto dto)
        {
            var result =
                await _userService.ResetPassword(dto);


            return Ok(result);
        }
    }
}