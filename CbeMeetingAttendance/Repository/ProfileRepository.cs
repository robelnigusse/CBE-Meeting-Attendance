using CbeMeetingAttendance.Data;
using CbeMeetingAttendance.Models;
using Microsoft.EntityFrameworkCore;

namespace CbeMeetingAttendance.Repositories
{
    public class ProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public ProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Profile?> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.Profiles
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.EmployeeId == employeeId);
        }

        public async Task<Profile?> GetByIdAsync(Guid id)
        {
            return await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Profile profile)
        {
            await _context.Profiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Profile profile)
        {
            _context.Profiles.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Profile profile)
        {
            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();
        }
    }
}