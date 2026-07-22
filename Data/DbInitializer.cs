using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace KNQARecruitmentPortal.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var context = (AppDbContext?)serviceProvider.GetService(typeof(AppDbContext));
            if (context == null) return;

            // Seed roles
            string[] roleNames = ["Admin", "Recruiter", "Applicant"];
            foreach (var roleName in roleNames)
            {
                if (!context.Roles.Any(r => r.RoleName == roleName))
                {
                    context.Roles.Add(new Models.Role { RoleName = roleName, Description = null, CreatedAt = DateTime.Now });
                }
            }

            // Example permissions
            var perms = new[]
            {
                "Applications.Review",
                "Vacancies.Create",
                "Users.Manage"
            };

            foreach (var p in perms)
            {
                if (!context.Permissions.Any(x => x.PermissionName == p))
                {
                    context.Permissions.Add(new Models.Permission { PermissionName = p });
                }
            }

            await context.SaveChangesAsync();

            // Assign some permissions to Admin role
            var adminRole = context.Roles.FirstOrDefault(r => r.RoleName == "Admin");
            if (adminRole != null)
            {
                foreach (var permName in perms)
                {
                    var perm = context.Permissions.FirstOrDefault(x => x.PermissionName == permName);
                    if (perm != null && !context.RolePermissions.Any(rp => rp.RoleId == adminRole.Id && rp.PermissionId == perm.Id))
                    {
                        context.RolePermissions.Add(new Models.RolePermission { RoleId = adminRole.Id, PermissionId = perm.Id });
                    }
                }
            }

            // Create admin user if missing
            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var adminUser = new Models.User
                {
                    FirstName = "System",
                    OtherName = "Administrator",
                    UserName = "admin",
                    EmailAddress = "admin@example.com",
                    PhoneNumber = null,
                    RoleId = adminRole != null ? adminRole.Id : 0,
                    Status = 1,
                    CreatedAt = DateTime.Now
                };

                var hasher = new PasswordHasher<Models.User>();
                adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

                context.Users.Add(adminUser);
            }

            if (context != null && !context.Vacancies.Any())
            {
                context.Vacancies.AddRange(
                    new Vacancy
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Title = "Director, Quality Assurance & Accreditation",
                        Department = "Accreditation and Standards Division",
                        Description = "Oversee national qualification framework metrics, system registration audits, and institutional compliance standards.",
                        ClosingDate = DateTime.Now.AddDays(14),
                        Status = 1,
                        CreatedAt = DateTime.Now
                    },
                    new Vacancy
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Title = "Senior Qualifications Framework Officer",
                        Department = "Technical Frameworks Portfolio",
                        Description = "Perform peer evaluation reviews on foreign standard validations and log credential authentication requests.",
                        ClosingDate = DateTime.Now.AddDays(7),
                        Status = 1,
                        CreatedAt = DateTime.Now
                    }
                );

                if (!context.JobApplications.Any())
                {
                    context.JobApplications.Add(new JobApplication
                    {
                        Id = Guid.NewGuid(),
                        VacancyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        ApplicantUserId = Guid.NewGuid(),
                        SubmittedAt = DateTime.Now.AddDays(-2),
                        Status = 0
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
