using Microsoft.EntityFrameworkCore;
using CbeMeetingAttendance.Data;
using CbeMeetingAttendance.Enums;
using CbeMeetingAttendance.Models;

namespace CbeMeetingAttendance.Repositories
{
    public class AttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Attendance> AddAttendanceAsync(Attendance attendance)
        {
            _context.Attendances.Add(attendance);

            await _context.SaveChangesAsync();

            return attendance;
        }

        public async Task<bool> AttendanceExistsAsync(
            Guid employeeId,
            DateOnly meetingDate,
            Session session)
        {
            return await _context.Attendances.AnyAsync(a =>
                a.EmployeeId == employeeId &&
                a.MeetingDate == meetingDate &&
                a.Session == session);
        }

        public async Task<List<Attendance>> GetEmployeeAttendanceAsync(Guid employeeId)
        {
            return await _context.Attendances
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.MeetingDate)
                .ToListAsync();
        }
    }
}