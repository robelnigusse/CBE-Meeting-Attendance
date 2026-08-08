using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CbeMeetingAttendance.DTOs;
using CbeMeetingAttendance.Enums;
using CbeMeetingAttendance.Models;
using CbeMeetingAttendance.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CbeMeetingAttendance.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly RefreshTokenRepository _refreshTokenRepository;
        private readonly EmployeeRepository _employeeRepository;
        private readonly JwtSettings _jwtSettings;


        public UserService(
            UserRepository userRepository,
            RefreshTokenRepository refreshTokenRepository,
            EmployeeRepository employeeRepository,
            IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _employeeRepository = employeeRepository;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<ApiResponse<bool>> RegisterUser(
    UserRegisterDto dto)
        {
            var existingUser =
    await _userRepository.GetByEmailAsync(dto.Email);


            if (existingUser != null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User already exists"
                };
            }


            // Find employee
            var employee =
                await _employeeRepository
                    .GetByEmployeeIdAsync(dto.EmployeeId!);


            if (employee == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Employee not found"
                };
            }



            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,

                // IMPORTANT:
                // this is Employee table Id (Guid)
                EmployeeId = employee.Id
            };



            var result =
                await _userRepository.CreateAsync(
                    user,
                    dto.Password);



            if (!result.Succeeded)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = string.Join(", ",
                        result.Errors.Select(e => e.Description))
                };
            }



            await _userRepository.AddRoleAsync(
                user,
                "Staff");



            return new ApiResponse<bool>
            {
                Success = true,
                Message = "User registered successfully",
                Data = true
            };
        }
        private async Task<string> GenerateJwtToken(
    User user)
        {
            var roles =
                await _userRepository.GetRolesAsync(user);


            var claims = new List<Claim>
    {
        new Claim(
            JwtRegisteredClaimNames.Sub,
            user.Id.ToString()),


        new Claim(
            JwtRegisteredClaimNames.Email,
            user.Email!),


        new Claim(
            "EmployeeId",
            user.EmployeeId?.ToString() ?? "")
    };


            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        role));
            }


            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _jwtSettings.Key));


            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);


            var token =
                new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audiences[0],
                    claims: claims,
                    expires:
                        DateTime.UtcNow.AddMinutes(
                            _jwtSettings.ExpiryMinutes),
                    signingCredentials: credentials);


            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
        private RefreshToken CreateRefreshToken(
    Guid userId)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),

                Token =
                    Convert.ToBase64String(
                        RandomNumberGenerator.GetBytes(64)),

                UserId = userId,

                CreatedAt = DateTime.UtcNow,

                ExpiryDate =
                    DateTime.UtcNow.AddDays(7),

                IsRevoked = false
            };
        }
        public async Task<ApiResponse<AuthResponseDto>> Login(
    LoginDto dto)
        {
            var user =
                await _userRepository.GetByEmailAsync(dto.Email);


            if (user == null)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }


            if (await _userRepository.IsLockedOutAsync(user))
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = "Account is locked. Try again later."
                };
            }


            var passwordCorrect =
                await _userRepository.CheckPasswordAsync(
                    user,
                    dto.Password);


            if (!passwordCorrect)
            {
                await _userRepository.AccessFailedAsync(user);


                return new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }


            await _userRepository.ResetAccessFailedCountAsync(user);


            var accessToken =
                await GenerateJwtToken(user);


            var refreshToken =
                CreateRefreshToken(user.Id);


            await _refreshTokenRepository
                .AddAsync(refreshToken);



            var roles =
                await _userRepository.GetRolesAsync(user);


            Employee? employee = null;


            if (user.EmployeeId.HasValue)
            {
                employee =
                    await _employeeRepository
                    .GetByIdAsync(
                        user.EmployeeId.Value);
            }



            return new ApiResponse<AuthResponseDto>
            {
                Success = true,

                Data = new AuthResponseDto
                {
                    Token = accessToken,

                    Expiration =
                        DateTime.UtcNow.AddMinutes(
                            _jwtSettings.ExpiryMinutes),

                    User = new CurrentUserDto
                    {
                        Id = user.Id,

                        Email = user.Email!,

                        EmployeeId =
                            user.EmployeeId,

                        EmployeeName =
                            employee?.FullName,

                        Division =
                            employee?.Division,

                        Department =
                            employee?.Department,

                        JobTitle =
                            employee?.JobTitle,

                        Roles = roles
                    }
                }
            };
        }
        public async Task<ApiResponse<TokenResponseDto>> Refresh(
    RefreshTokenRequestDto dto)
        {
            var storedToken =
                await _refreshTokenRepository.GetAsync(
                    dto.RefreshToken);


            if (storedToken == null)
            {
                return new ApiResponse<TokenResponseDto>
                {
                    Success = false,
                    Message = "Invalid refresh token"
                };
            }


            if (storedToken.IsRevoked)
            {
                return new ApiResponse<TokenResponseDto>
                {
                    Success = false,
                    Message = "Refresh token revoked"
                };
            }


            if (storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return new ApiResponse<TokenResponseDto>
                {
                    Success = false,
                    Message = "Refresh token expired"
                };
            }


            var user = storedToken.User;


            var newAccessToken =
                await GenerateJwtToken(user);


            var newRefreshToken =
                CreateRefreshToken(user.Id);


            storedToken.IsRevoked = true;


            await _refreshTokenRepository
                .AddAsync(newRefreshToken);


            await _refreshTokenRepository.UpdateAsync();



            return new ApiResponse<TokenResponseDto>
            {
                Success = true,

                Data = new TokenResponseDto
                {
                    AccessToken = newAccessToken,

                    RefreshToken =
                        newRefreshToken.Token,

                    AccessTokenExpiration =
                        DateTime.UtcNow.AddMinutes(
                            _jwtSettings.ExpiryMinutes),

                    RefreshTokenExpiration =
                        newRefreshToken.ExpiryDate
                }
            };
        }
        public async Task<ApiResponse<bool>> Logout(
    RefreshTokenRequestDto dto)
        {
            var token =
                await _refreshTokenRepository
                .GetAsync(dto.RefreshToken);


            if (token == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Token not found"
                };
            }


            token.IsRevoked = true;


            await _refreshTokenRepository
                .SaveChangesAsync();


            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Logout successful",
                Data = true
            };
        }
        public async Task<ApiResponse<bool>> AssignRole(
    Guid userId,
    string role,
    ClaimsPrincipal currentUser)
        {
            var creatorRole =
                currentUser
                .FindFirst(ClaimTypes.Role)?
                .Value;


            if (creatorRole == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Unauthorized"
                };
            }


            if (!RoleHierarchy.Levels.ContainsKey(role))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid role"
                };
            }


            if (RoleHierarchy.Levels[creatorRole]
               <= RoleHierarchy.Levels[role])
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message =
                    "You cannot assign this role"
                };
            }


            var user =
                await _userRepository
                .GetByIdAsync(userId);


            if (user == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User not found"
                };
            }


            await _userRepository
                .AddRoleAsync(user, role);


            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Role assigned",
                Data = true
            };
        }
        public async Task<ApiResponse<bool>> ChangeRole(
    ChangeRoleDto dto,
    ClaimsPrincipal currentUser)
        {
            var currentUserId =
    Guid.Parse(
        currentUser.FindFirst(
            ClaimTypes.NameIdentifier)!
            .Value);


            if (currentUserId == dto.UserId)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "You cannot change your own role"
                };
            }
            var creatorRole =
                currentUser
                .FindFirst(ClaimTypes.Role)?
                .Value;


            if (creatorRole == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Unauthorized"
                };
            }


            if (!RoleHierarchy.Levels.ContainsKey(dto.NewRole))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid role"
                };
            }


            // hierarchy check
            if (RoleHierarchy.Levels[creatorRole]
                <= RoleHierarchy.Levels[dto.NewRole])
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message =
                    "You cannot assign this role"
                };
            }



            var user =
                await _userRepository
                .GetByIdAsync(dto.UserId);


            if (user == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User not found"
                };
            }



            var oldRoles =
                await _userRepository
                .GetRolesAsync(user);



            foreach (var role in oldRoles)
            {
                await _userRepository
                    .RemoveRoleAsync(user, role);
            }



            await _userRepository
                .AddRoleAsync(
                    user,
                    dto.NewRole);



            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Role changed successfully",
                Data = true
            };
        }
        public async Task<ApiResponse<bool>> RemoveRole(
    Guid userId,
    string role,
    ClaimsPrincipal currentUser)
        {
            var currentUserId =
    Guid.Parse(
        currentUser.FindFirst(
            ClaimTypes.NameIdentifier)!
            .Value);


            if (currentUserId == userId)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message =
                    "You cannot remove your own role"
                };
            }

            var creatorRole =
                currentUser
                .FindFirst(ClaimTypes.Role)?
                .Value;


            if (creatorRole == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Unauthorized"
                };
            }


            if (!RoleHierarchy.Levels.ContainsKey(role))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid role"
                };
            }



            if (RoleHierarchy.Levels[creatorRole]
                <= RoleHierarchy.Levels[role])
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message =
                    "You cannot remove this role"
                };
            }



            var user =
                await _userRepository
                .GetByIdAsync(userId);



            if (user == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User not found"
                };
            }



            var result =
                await _userRepository
                .RemoveRoleAsync(user, role);



            return new ApiResponse<bool>
            {
                Success = result.Succeeded,

                Message = result.Succeeded
                    ? "Role removed"
                    : "Failed removing role",

                Data = result.Succeeded
            };
        }
        public async Task<ApiResponse<bool>> ChangePassword(
    Guid userId,
    ChangePasswordDto dto)
        {
            var user =
                await _userRepository
                .GetByIdAsync(userId);


            if (user == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User not found"
                };
            }


            var result =
                await _userRepository
                .ChangePasswordAsync(
                    user,
                    dto.OldPassword,
                    dto.NewPassword);



            return new ApiResponse<bool>
            {
                Success = result.Succeeded,

                Message = result.Succeeded
                ? "Password changed successfully"
                : string.Join(", ",
                    result.Errors.Select(e => e.Description)),

                Data = result.Succeeded
            };
        }
        public async Task<ApiResponse<bool>> ResetPassword(
    ResetPasswordDto dto)
        {
            var user =
                await _userRepository
                .GetByIdAsync(dto.UserId);


            if (user == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User not found"
                };
            }


            var result =
                await _userRepository
                .ResetPasswordAsync(
                    user,
                    dto.Token,
                    dto.NewPassword);



            return new ApiResponse<bool>
            {
                Success = result.Succeeded,

                Message = result.Succeeded
                ? "Password reset successful"
                : "Password reset failed",

                Data = result.Succeeded
            };
        }
        public async Task<ApiResponse<CurrentUserDto>> GetUserById(
    Guid id)
        {
            var user =
                await _userRepository
                .GetByIdAsync(id);


            if (user == null)
            {
                return new ApiResponse<CurrentUserDto>
                {
                    Success = false,
                    Message = "User not found"
                };
            }


            var roles =
                await _userRepository
                .GetRolesAsync(user);


            return new ApiResponse<CurrentUserDto>
            {
                Success = true,

                Data = new CurrentUserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    EmployeeId = user.EmployeeId,
                    Roles = roles
                }
            };
        }
        public async Task<ApiResponse<List<CurrentUserDto>>> GetAllUsers()
        {
            var users =
                await _userRepository
                .GetAllAsync();


            var result =
                new List<CurrentUserDto>();


            foreach (var user in users)
            {
                result.Add(
                    new CurrentUserDto
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        EmployeeId = user.EmployeeId,

                        Roles =
                            await _userRepository
                            .GetRolesAsync(user)
                    });
            }


            return new ApiResponse<List<CurrentUserDto>>
            {
                Success = true,
                Total = result.Count,
                Data = result
            };
        }
    }
}