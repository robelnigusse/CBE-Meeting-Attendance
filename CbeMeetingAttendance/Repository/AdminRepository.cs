using Microsoft.EntityFrameworkCore;
using CbeMeetingAttendance.Data;
using CbeMeetingAttendance.Enums;
using CbeMeetingAttendance.Models;

namespace CbeMeetingAttendance.Repositories
{
    public class AdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalEmployeesAsync()
        {
            return await _context.Employees.CountAsync();
        }

        public async Task<int> GetTodayAttendanceCountAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Attendances
                .CountAsync(a => a.MeetingDate == today);
        }

        public async Task<int> GetMorningAttendanceCountAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Attendances
                .CountAsync(a =>
                    a.MeetingDate == today &&
                    a.Session == Session.Morning);
        }

        public async Task<int> GetAfternoonAttendanceCountAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Attendances
                .CountAsync(a =>
                    a.MeetingDate == today &&
                    a.Session == Session.Afternoon);
        }

        public async Task<List<Attendance>> GetTodayAttendanceAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.MeetingDate == today)
                .OrderBy(a => a.AttendanceTime)
                .ToListAsync();
        }
    }
}