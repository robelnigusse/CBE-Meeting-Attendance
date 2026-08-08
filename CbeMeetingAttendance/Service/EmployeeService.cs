using CbeMeetingAttendance.DTOs;
using CbeMeetingAttendance.DTOs.Employee;
using CbeMeetingAttendance.DTOs.Profile;
using CbeMeetingAttendance.Mappers;
using CbeMeetingAttendance.Repositories;

namespace CbeMeetingAttendance.Services
{
    public class EmployeeService
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly ProfileService _profileService;
        public EmployeeService(EmployeeRepository employeeRepository, ProfileService profileService)
        {
            _employeeRepository = employeeRepository;
            _profileService = profileService;
        }

        // Register Employee
        public async Task<ApiResponse<EmployeeResponseDto>> RegisterEmployeeAsync(RegisterEmployeeDto dto)
        {
            var exists = await _employeeRepository.EmployeeExistsAsync(dto.EmployeeId);

            if (exists)
            {
                return new ApiResponse<EmployeeResponseDto>
                {
                    Success = false,
                    Message = "Employee already exists."
                };
            }

            var employee = EmployeeMapper.ToEmployee(dto);

            employee.Id = Guid.NewGuid();
            employee.CreatedAt = DateTime.UtcNow;

            employee = await _employeeRepository.RegisterAsync(employee);


            if (dto.Image != null)
            {
                var profileDto = new UploadProfileImageDto
                {
                    EmployeeId = employee.EmployeeId,
                    Image = dto.Image
                };


                var profileResult =
                    await _profileService.UploadProfileImageAsync(profileDto);


                if (!profileResult.Success)
                {
                    return new ApiResponse<EmployeeResponseDto>
                    {
                        Success = false,
                        Message = profileResult.Message
                    };
                }
            }

            return new ApiResponse<EmployeeResponseDto>
            {
                Success = true,
                Message = "Employee registered successfully.",
                Data = EmployeeMapper.ToEmployeeResponseDto(employee)
            };
        }

        // Get All Employees
        public async Task<ApiResponse<List<EmployeeResponseDto>>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            return new ApiResponse<List<EmployeeResponseDto>>
            {
                Success = true,
                Message = "Employees retrieved successfully.",
                Data = EmployeeMapper.ToEmployeeResponseDtoList(employees)
            };
        }
        public async Task<EmployeeResponseDto?> GetEmployeeByEmployeeIdAsync(string employeeId)
        {
            var employee = await _employeeRepository.GetByEmployeeIdAsync(employeeId);

            if (employee == null)
            {
                return null;
            }

            return EmployeeMapper.ToEmployeeResponseDto(employee);
        }

        // Get Employee By Guid Id
        public async Task<ApiResponse<EmployeeResponseDto>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                return new ApiResponse<EmployeeResponseDto>
                {
                    Success = false,
                    Message = "Employee not found."
                };
            }

            return new ApiResponse<EmployeeResponseDto>
            {
                Success = true,
                Message = "Employee found.",
                Data = EmployeeMapper.ToEmployeeResponseDto(employee)
            };
        }

        // Update Employee
        public async Task<ApiResponse<EmployeeResponseDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                return new ApiResponse<EmployeeResponseDto>
                {
                    Success = false,
                    Message = "Employee not found."
                };
            }

            EmployeeMapper.UpdateEmployee(employee, dto);

            employee = await _employeeRepository.UpdateAsync(employee);

            return new ApiResponse<EmployeeResponseDto>
            {
                Success = true,
                Message = "Employee updated successfully.",
                Data = EmployeeMapper.ToEmployeeResponseDto(employee)
            };
        }

        // Delete Employee
        public async Task<ApiResponse<bool>> DeleteEmployeeAsync(Guid id)
        {
            var deleted = await _employeeRepository.DeleteAsync(id);

            return new ApiResponse<bool>
            {
                Success = deleted,
                Message = deleted
                    ? "Employee deleted successfully."
                    : "Employee not found.",
                Data = deleted
            };
        }
    }
}