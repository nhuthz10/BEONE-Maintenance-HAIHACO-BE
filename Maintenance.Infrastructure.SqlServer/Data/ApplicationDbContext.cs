using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maintenance.Infrastructure.SqlServer.Entities;

namespace Maintenance.Infrastructure.SqlServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserDevices> UserDevices { get; set; }
        public DbSet<NotificationLogs> NotificationLogs { get; set; }
        public DbSet<Equipments> Equipments { get; set; }
        public DbSet<EquipmentCheckLists> EquipmentCheckLists { get; set; }
        public DbSet<EquipmentSpareParts> EquipmentSpareParts { get; set; }
        public DbSet<Maintenances> Maintenances { get; set; }
        public DbSet<MaintenanceDocs> MaintenanceDocs { get; set; }
        public DbSet<MaintenanceDocDetails> MaintenanceDocDetails { get; set; }
        public DbSet<MaintenenceAttachments> MaintenenceAttachments { get; set; }
        public DbSet<MaintenenceCheckLists> MaintenenceCheckLists { get; set; }
        public DbSet<MaintenenceSpareParts> MaintenenceSpareParts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Maintenances>()
            .Property(x => x.DocNo)
            .HasComputedColumnSql(
                @"'W-O' +
                  CASE
                      WHEN Id < 100000
                          THEN RIGHT('00000' + CAST(Id AS VARCHAR(20)), 5)
                      ELSE CAST(Id AS VARCHAR(20))
                  END",
                stored: true);

            SeedRoleAndUser(builder);
        }

        private void SeedRoleAndUser(ModelBuilder builder)
        {
            var technicalRoleId = Guid.NewGuid().ToString();
            var userRoleId = Guid.NewGuid().ToString();
            var adminRoleId = Guid.NewGuid().ToString();

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
                new IdentityRole { Id = technicalRoleId, Name = "Technical", ConcurrencyStamp = "2", NormalizedName = "TECHNICAL" },
                new IdentityRole { Id = userRoleId, Name = "User", ConcurrencyStamp = "3", NormalizedName = "USER" }
            );

            var adminUserId = Guid.NewGuid().ToString();
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();

            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                FullName = "Supper Admin",
                PhoneNumber = "0123456789",
                EmailConfirmed = true,
                LockoutEnabled = false,
                UserName = "SupperAdmin",
                NormalizedUserName = "SUPPERADMIN",
                Email = "supperadmin.sys@system.vn",
                PasswordHash = passwordHasher.HashPassword(null, "123456aA@"),
                IsActive = true,
            };

            builder.Entity<ApplicationUser>().HasData(adminUser);

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminUserId }
            );
        }
    }
}
