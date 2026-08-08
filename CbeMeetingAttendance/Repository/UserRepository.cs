using CbeMeetingAttendance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CbeMeetingAttendance.Repositories
{
    public class UserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserRepository(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }


        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }


        public async Task<IdentityResult> CreateAsync(
            User user,
            string password)
        {
            return await _userManager.CreateAsync(
                user,
                password);
        }


        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }


        public async Task<IdentityResult> AddRoleAsync(
            User user,
            string role)
        {
            return await _userManager.AddToRoleAsync(
                user,
                role);
        }


        public async Task<IdentityResult> RemoveRoleAsync(
            User user,
            string role)
        {
            return await _userManager.RemoveFromRoleAsync(
                user,
                role);
        }


        public async Task<IdentityResult> ChangePasswordAsync(
            User user,
            string oldPassword,
            string newPassword)
        {
            return await _userManager.ChangePasswordAsync(
                user,
                oldPassword,
                newPassword);
        }


        public async Task<string> GeneratePasswordResetTokenAsync(
            User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }


        public async Task<IdentityResult> ResetPasswordAsync(
            User user,
            string token,
            string newPassword)
        {
            return await _userManager.ResetPasswordAsync(
                user,
                token,
                newPassword);
        }


        public async Task<bool> CheckPasswordAsync(
            User user,
            string password)
        {
            return await _userManager.CheckPasswordAsync(
                user,
                password);
        }


        public async Task<IdentityResult> UpdateAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }


        public async Task<bool> RoleExistsAsync(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }
        public async Task<bool> IsLockedOutAsync(User user)
        {
            return await _userManager.IsLockedOutAsync(user);
        }


        public async Task<IdentityResult> AccessFailedAsync(User user)
        {
            return await _userManager.AccessFailedAsync(user);
        }


        public async Task ResetAccessFailedCountAsync(User user)
        {
            await _userManager.ResetAccessFailedCountAsync(user);
        }
        public async Task<List<User>> GetAllAsync()
        {
            return await _userManager.Users
                .ToListAsync();
        }
    }
}