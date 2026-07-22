using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection.Emit;

namespace KNQARecruitmentPortal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Models.WorkExperience> WorkExperiences
        { get; set; } = default!;
        // RBAC entities
        public DbSet<Models.Role> Roles { get; set; } = default!;
        public DbSet<Models.Permission> Permissions { get; set; } = default!;
        public DbSet<Models.RolePermission> RolePermissions { get; set; } = default!;
        public DbSet<Models.User> Users { get; set; } = default!;
        public DbSet<Vacancy> Vacancies { get; set; } = default!;
        public DbSet<ApplicantProfile> ApplicantProfiles { get; set; } = default!;
        public DbSet<JobApplication> JobApplications { get; set; } = default!;
        public DbSet<ApplicationQualification> ApplicationQualifications { get; set; } = default!;
        public DbSet<ApplicationDocument> ApplicationDocuments { get; set; } = default!;
        public DbSet<ApplicationReview> ApplicationReviews { get; set; } = default!;
        public DbSet<InterviewSlot> InterviewSlots { get; set; } = default!;
        public DbSet<NotificationLog> NotificationLogs { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

       
            modelBuilder.Entity<Vacancy>().Property<Guid>("VacancyId");
            modelBuilder.Entity<Vacancy>().HasKey("VacancyId");

            modelBuilder.Entity<ApplicantProfile>().Property<Guid>("ApplicantProfileId");
            modelBuilder.Entity<ApplicantProfile>().HasKey("ApplicantProfileId");

            modelBuilder.Entity<JobApplication>().Property<Guid>("JobApplicationId");
            modelBuilder.Entity<JobApplication>().HasKey("JobApplicationId");

            modelBuilder.Entity<ApplicationQualification>().Property<Guid>("ApplicationQualificationId");
            modelBuilder.Entity<ApplicationQualification>().HasKey("ApplicationQualificationId");

            modelBuilder.Entity<ApplicationDocument>().Property<Guid>("ApplicationDocumentId");
            modelBuilder.Entity<ApplicationDocument>().HasKey("ApplicationDocumentId");

            modelBuilder.Entity<ApplicationReview>().Property<Guid>("ApplicationReviewId");
            modelBuilder.Entity<ApplicationReview>().HasKey("ApplicationReviewId");

            modelBuilder.Entity<InterviewSlot>().Property<Guid>("InterviewSlotId");
            modelBuilder.Entity<InterviewSlot>().HasKey("InterviewSlotId");

            modelBuilder.Entity<NotificationLog>().Property<Guid>("NotificationLogId");
            modelBuilder.Entity<NotificationLog>().HasKey("NotificationLogId");
            modelBuilder.Entity<Models.WorkExperience>(entity =>
            {
                entity.HasKey(e => e.ExperienceId);
                entity.ToTable("WorkExperiences");
            });

            // RBAC mapping to match existing database schema
            modelBuilder.Entity<Models.Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RoleName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Models.Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PermissionName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<Models.RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");
                entity.HasKey(e => new { e.RoleId, e.PermissionId });
                entity.HasOne(rp => rp.Role).WithMany().HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(rp => rp.Permission).WithMany().HasForeignKey(rp => rp.PermissionId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Models.User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.OtherName).HasMaxLength(50);
                entity.Property(e => e.UserName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.EmailAddress).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Status).HasDefaultValue((byte)1);
                entity.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId);
            });
        }
    }


    public class Vacancy { public Guid Id { get; set; } = Guid.NewGuid(); public string Title { get; set; } = ""; public string Description { get; set; } = ""; public string Department { get; set; } = ""; public DateTime ClosingDate { get; set; } = DateTime.Now.AddDays(14); public int Status { get; set; } public Guid CreatedByUserId { get; set; } public DateTime CreatedAt { get; set; } = DateTime.Now; }
    public class ApplicantProfile { public Guid Id { get; set; } = Guid.NewGuid(); public Guid UserId { get; set; } public DateTime DateOfBirth { get; set; } public string PhoneNumber { get; set; } = ""; public string Address { get; set; } = ""; public string NationalIdNumber { get; set; } = ""; public string ProfilePhotoPath { get; set; } = ""; }
    public class JobApplication { public Guid Id { get; set; } = Guid.NewGuid(); public Guid VacancyId { get; set; } public Guid ApplicantUserId { get; set; } public DateTime SubmittedAt { get; set; } = DateTime.Now; public int Status { get; set; } }
    public class ApplicationQualification { public Guid Id { get; set; } = Guid.NewGuid(); public Guid JobApplicationId { get; set; } public string QualificationName { get; set; } = ""; public string Institution { get; set; } = ""; public int YearObtained { get; set; } }
    public class ApplicationDocument { public Guid Id { get; set; } = Guid.NewGuid(); public Guid JobApplicationId { get; set; } public int DocumentType { get; set; } public string FilePath { get; set; } = ""; public DateTime UploadedAt { get; set; } = DateTime.Now; public long FileSize { get; set; } }
    public class ApplicationReview { public Guid Id { get; set; } = Guid.NewGuid(); public Guid JobApplicationId { get; set; } public Guid ReviewerUserId { get; set; } public int Score { get; set; } public string Comments { get; set; } = ""; public DateTime ReviewedAt { get; set; } = DateTime.Now; }
    public class InterviewSlot { public Guid Id { get; set; } = Guid.NewGuid(); public Guid JobApplicationId { get; set; } public DateTime ScheduledAt { get; set; } = DateTime.Now; public string LocationOrLink { get; set; } = ""; public Guid InterviewerUserId { get; set; } public int Status { get; set; } }
    public class NotificationLog { public Guid Id { get; set; } = Guid.NewGuid(); public Guid RecipientUserId { get; set; } public string Subject { get; set; } = ""; public DateTime SentAt { get; set; } = DateTime.Now; public string Channel { get; set; } = "Email"; public bool Success { get; set; } = true; }
}
