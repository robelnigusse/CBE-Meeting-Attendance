using CbeMeetingAttendance.DTOs;
using CbeMeetingAttendance.DTOs.Profile;
using CbeMeetingAttendance.Models;
using CbeMeetingAttendance.Repositories;

namespace CbeMeetingAttendance.Services
{
    public class ProfileService
    {
        private readonly ProfileRepository _profileRepository;
        private readonly EmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment _environment;

        public ProfileService(
            ProfileRepository profileRepository,
            EmployeeRepository employeeRepository,
            IWebHostEnvironment environment)
        {
            _profileRepository = profileRepository;
            _employeeRepository = employeeRepository;
            _environment = environment;
        }

        public async Task<ApiResponse<ProfileResponseDto>> UploadProfileImageAsync(
            UploadProfileImageDto dto)
        {
            var response = new ApiResponse<ProfileResponseDto>();

            // Check employee exists
            var employee =
                await _employeeRepository.GetByEmployeeIdAsync(dto.EmployeeId);

            if (employee == null)
            {
                response.Success = false;
                response.Message = "Employee not found";
                return response;
            }

            // Validate image
            if (dto.Image == null || dto.Image.Length == 0)
            {
                response.Success = false;
                response.Message = "Please select an image";
                return response;
            }

            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png"
            };

            var extension =
                Path.GetExtension(dto.Image.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                response.Success = false;
                response.Message =
                    "Only jpg, jpeg and png images are allowed";
                return response;
            }

            // Maximum 5 MB
            if (dto.Image.Length > 5 * 1024 * 1024)
            {
                response.Success = false;
                response.Message =
                    "Image size must not exceed 5 MB";
                return response;
            }

            // Create folder if it doesn't exist
            var folder =
                Path.Combine(
                    _environment.WebRootPath,
                    "ProfileImages");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Generate unique filename
            var fileName =
                $"{Guid.NewGuid()}{extension}";

            var filePath =
                Path.Combine(folder, fileName);

            // Convert image to byte[]
            byte[] imageBytes;

            using (var memoryStream = new MemoryStream())
            {
                await dto.Image.CopyToAsync(memoryStream);

                imageBytes = memoryStream.ToArray();
            }

            // Save image to local storage
            await File.WriteAllBytesAsync(
                filePath,
                imageBytes);

            // Check if profile already exists
            var existingProfile =
                await _profileRepository.GetByEmployeeIdAsync(employee.Id);

            if (existingProfile == null)
            {
                var profile = new Profile
                {
                    EmployeeId = employee.Id,
                    FileName = dto.Image.FileName,
                    ContentType = dto.Image.ContentType,
                    Path = filePath,
                    ImageData = imageBytes,
                    UploadedAt = DateTime.UtcNow
                };

                await _profileRepository.AddAsync(profile);

                response.Success = true;
                response.Message = "Profile image uploaded successfully";
                response.Data = new ProfileResponseDto
                {
                    EmployeeId = profile.EmployeeId,
                    FileName = profile.FileName,
                    Path = profile.Path,
                    UploadedAt = profile.UploadedAt
                };

                return response;
            }

            // Delete old local image
            if (File.Exists(existingProfile.Path))
            {
                File.Delete(existingProfile.Path);
            }

            // Update database
            existingProfile.FileName = dto.Image.FileName;
            existingProfile.ContentType = dto.Image.ContentType;
            existingProfile.Path = filePath;
            existingProfile.ImageData = imageBytes;
            existingProfile.UploadedAt = DateTime.UtcNow;

            await _profileRepository.UpdateAsync(existingProfile);

            response.Success = true;
            response.Message = "Profile image updated successfully";
            response.Data = new ProfileResponseDto
            {
                EmployeeId = existingProfile.EmployeeId,
                FileName = existingProfile.FileName,
                Path = existingProfile.Path,
                UploadedAt = existingProfile.UploadedAt
            };

            return response;
        }

        public async Task<byte[]?> GetProfileImageAsync(string employeeId)
        {
            var employee =
                await _employeeRepository.GetByEmployeeIdAsync(employeeId);

            if (employee == null)
                return null;

            var profile =
                await _profileRepository.GetByEmployeeIdAsync(employee.Id);

            return profile?.ImageData;
        }

        public async Task<ApiResponse<bool>> DeleteProfileImageAsync(
            string employeeId)
        {
            var response = new ApiResponse<bool>();

            var employee =
                await _employeeRepository.GetByEmployeeIdAsync(employeeId);

            if (employee == null)
            {
                response.Success = false;
                response.Message = "Employee not found";
                response.Data = false;
                return response;
            }

            var profile =
                await _profileRepository.GetByEmployeeIdAsync(employee.Id);

            if (profile == null)
            {
                response.Success = false;
                response.Message = "Profile image not found";
                response.Data = false;
                return response;
            }

            if (File.Exists(profile.Path))
            {
                File.Delete(profile.Path);
            }

            await _profileRepository.DeleteAsync(profile);

            response.Success = true;
            response.Message = "Profile image deleted successfully";
            response.Data = true;

            return response;
        }
    }
}