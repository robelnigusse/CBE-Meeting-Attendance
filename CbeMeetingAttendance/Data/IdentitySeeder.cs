using CbeMeetingAttendance.Models;
using CbeMeetingAttendance.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CbeMeetingAttendance.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();

            var context =
                serviceProvider.GetRequiredService<ApplicationDbContext>();


            // Roles
            string[] roles =
            {
                "SuperAdmin",
                "Admin",
                "VP",
                "Director",
                "Manager",
                "Staff"
            };


            // Create roles
            foreach (var role in roles)
            {
                var roleExists =
                    await roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    await roleManager.CreateAsync(
                        new IdentityRole<Guid>(role));
                }
            }



            var adminEmail = "superadmin@cbe.com";


            // Check if admin employee exists
            var adminEmployee =
                await context.Employees
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == "ADM001");


            if (adminEmployee == null)
            {
                adminEmployee = new Employee
                {
                    Id = Guid.NewGuid(),

                    EmployeeId = "ADM001",

                    FullName = "Super Admin",

                    Division = "Administration",

                    Department = "IT",

                    JobTitle = "System Administrator",

                    PhoneNumber = "0000000000",

                    CreatedAt = DateTime.UtcNow
                };


                context.Employees.Add(adminEmployee);

                await context.SaveChangesAsync();
            }



            // Check if SuperAdmin user exists
            var existingAdmin =
                await userManager.FindByEmailAsync(adminEmail);



            if (existingAdmin == null)
            {
                var adminUser = new User
                {
                    UserName = adminEmail,

                    Email = adminEmail,

                    EmailConfirmed = true,

                    EmployeeId = adminEmployee.Id
                };


                var result =
                    await userManager.CreateAsync(
                        adminUser,
                        "Admin@123");


                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        adminUser,
                        "SuperAdmin");
                }
                else
                {
                    throw new Exception(
                        string.Join(
                            ", ",
                            result.Errors.Select(e => e.Description)
                        ));
                }
            }
            else
            {
                // Safety update:
                // If user exists but EmployeeId is missing
                if (existingAdmin.EmployeeId == null)
                {
                    existingAdmin.EmployeeId =
                        adminEmployee.Id;


                    await userManager.UpdateAsync(
                        existingAdmin);
                }
            }
        }
    }
}