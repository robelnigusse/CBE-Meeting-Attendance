using CbeMeetingAttendance.DTOs;
using CbeMeetingAttendance.DTOs.Admin;
using CbeMeetingAttendance.Mappers;
using CbeMeetingAttendance.Repositories;

namespace CbeMeetingAttendance.Services
{
    public class AdminService
    {
        private readonly AdminRepository _adminRepository;

        public AdminService(AdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<ApiResponse<DashboardDto>> GetDashboardAsync()
        {
            var dashboard = new DashboardDto
            {
                TotalEmployees = await _adminRepository.GetTotalEmployeesAsync(),

                TodayAttendance = await _adminRepository.GetTodayAttendanceCountAsync(),

                MorningAttendance = await _adminRepository.GetMorningAttendanceCountAsync(),

                AfternoonAttendance = await _adminRepository.GetAfternoonAttendanceCountAsync()
            };

            var attendances = await _adminRepository.GetTodayAttendanceAsync();

            dashboard.TodayAttendees =
                AdminMapper.ToAttendanceSummaryDtoList(attendances);

            return new ApiResponse<DashboardDto>
            {
                Success = true,
                Message = "Dashboard loaded successfully.",
                Data = dashboard
            };
        }
    }
}