using CbeMeetingAttendance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CbeMeetingAttendance.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // EmployeeId should be unique
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeId)
                .IsUnique();
            modelBuilder.Entity<RefreshToken>()
    .HasOne(r => r.User)
    .WithMany()
    .HasForeignKey(r => r.UserId)
    .OnDelete(DeleteBehavior.Cascade);
            // Composite Unique Index
            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new
                {
                    a.EmployeeId,
                    a.MeetingDate,
                    a.Session
                })
                .IsUnique();

            // One Employee -> Many Attendances
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // One Employee -> One User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne()
                .HasForeignKey<User>(u => u.EmployeeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Profile>()
    .HasOne(p => p.Employee)
    .WithOne()
    .HasForeignKey<Profile>(p => p.EmployeeId)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Profile>()
                .HasIndex(p => p.EmployeeId)
                .IsUnique();
        }
    }
}