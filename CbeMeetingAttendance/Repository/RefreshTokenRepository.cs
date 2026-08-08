using CbeMeetingAttendance.Data;
using CbeMeetingAttendance.Models;
using Microsoft.EntityFrameworkCore;

namespace CbeMeetingAttendance.Repositories
{
    public class RefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetAsync(
            string token)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(
                    r => r.Token == token);
        }


        public async Task RevokeAsync(
            RefreshToken refreshToken)
        {
            refreshToken.IsRevoked = true;

            await _context.SaveChangesAsync();
        }
    }
}