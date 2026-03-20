using Domain.Entities;
using Domain.Entities.LooksUp;
using Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DBContext
{
    public class AppDBContext :IdentityDbContext<AppUser>
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<SavedJob> SavedJobs { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        // =========================
        // Identity & Users
        // =========================
        public DbSet<AppUser> AppUsers { get; set; }

        // =========================
        // Profiles
        // =========================
        public DbSet<JobSeeker> JobSeekers { get; set; }
        public DbSet<EmployerProfile> EmployerProfiles { get; set; }
        public DbSet<ProfileImage> ProfileImages { get; set; }
        public DbSet<Pdf> Pdfs { get; set; }
        public DbSet<SavedJobAlert> SavedJobAlerts { get; set; }
        // =========================
        // Jobs & Applications
        // =========================
        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<JobApplicationNote> JobApplicationNotes { get; set; }
        public DbSet<JobIndustry> JobIndustries { get; set; }
        public DbSet<JobStatus> JobStatuses { get; set; }
        public DbSet<WorkMode> WorkModes { get; set; }
        public DbSet<EmploymentType> EmploymentTypes { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Education> Educations { get; set; }

        public DbSet<Experience> Experiences { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /* =========================
               AppUser (Identity)
            ========================== */
            builder.Entity<AppUser>(entity =>
            {
                entity.Property(ep => ep.FullName)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(ep => ep.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(ep => ep.UpdatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
                // Optional navigation configuration (not strictly needed)
                entity.HasOne(u => u.JobSeeker)
                      .WithOne(js => js.User)
                      .HasForeignKey<JobSeeker>(js => js.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.EmployerProfile)
                      .WithOne(ep => ep.User)
                      .HasForeignKey<EmployerProfile>(ep => ep.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Experience>()
      .HasOne(e => e.JobSeeker)
      .WithMany(js => js.Experiences)
      .HasForeignKey(e => e.JobSeekerId)
      .OnDelete(DeleteBehavior.Cascade);

            // Education relationship
            builder.Entity<Education>()
                .HasOne(e => e.JobSeeker)
                .WithMany(js => js.Educations)
                .HasForeignKey(e => e.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);
        
        /* =========================
           JobSeeker
        ========================== */
        builder.Entity<JobSeeker>(entity =>
            {
                entity.ToTable("JobSeekers");

                entity.HasKey(js => js.Id);

                entity.Property(js => js.PhoneNumber)
                      .HasMaxLength(20);

                entity.HasMany(x => x.Skills)
                   .WithOne(s => s.JobSeeker)
                   .HasForeignKey(s => s.JobSeekerId)
                   .OnDelete(DeleteBehavior.Cascade); // or Restrict (see note below)


                entity.Property(js => js.ResumeUrl)
                      .HasMaxLength(500);

                entity.Property(js => js.Location)
                      .HasMaxLength(200);

                entity.HasOne(js => js.User)
                      .WithOne(u => u.JobSeeker)
                      .HasForeignKey<JobSeeker>(js => js.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(js => js.Resume) // one-to-one Resume
                      .WithOne(p => p.JobSeeker)
                      .HasForeignKey<Pdf>(p => p.JobSeekerId) // ✅ FK is on Pdf
                      .OnDelete(DeleteBehavior.Restrict);
            });

            /* =========================
               EmployerProfile
            ========================== */
            builder.Entity<EmployerProfile>(entity =>
            {
                entity.ToTable("EmployerProfiles");

                entity.HasKey(ep => ep.Id);

                entity.Property(ep => ep.CompanyName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(ep => ep.CompanyWebsite)
                      .HasMaxLength(300);

                entity.Property(ep => ep.Description)
                      .HasMaxLength(2000);

                entity.Property(ep => ep.Location)
                      .HasMaxLength(200);

                

                entity.HasOne(ep => ep.User)
                      .WithOne(u => u.EmployerProfile)
                      .HasForeignKey<EmployerProfile>(ep => ep.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            /* =========================
               ProfileImage
            ========================== */
            builder.Entity<ProfileImage>(entity =>
            {
                entity.ToTable("ProfileImages");

                entity.HasKey(pi => pi.Id);

                entity.Property(pi => pi.ImageUrl)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(pi => pi.PublicId)
                      .HasMaxLength(300);

                entity.Property(pi => pi.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

            
                    entity.HasOne(pi => pi.JobSeeker)
                          .WithOne(js => js.ProfileImage)
                          .HasForeignKey<ProfileImage>(pi => pi.JobSeekerId)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(pi => pi.EmployerProfile)
                          .WithOne(ep => ep.ProfileImage)
                          .HasForeignKey<ProfileImage>(pi => pi.EmployerProfileId)
                          .OnDelete(DeleteBehavior.Restrict);
                

            });
            builder.Entity<SavedJob>()
      .HasIndex(x => new { x.UserId, x.JobId })
      .IsUnique();

            //Pdf
            builder.Entity<Pdf>(entity =>
            {
                entity.ToTable("Pdfs");

                entity.HasKey(pi => pi.Id);

                entity.Property(pi => pi.PdfUrl)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(pi => pi.PublicId)
                      .HasMaxLength(300);

                entity.Property(pi => pi.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

            });
            builder.Entity<Skill>(entity =>
            {
                entity.ToTable("Skills");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasOne(x => x.JobSeeker)
                      .WithMany(js => js.Skills)
                      .HasForeignKey(x => x.JobSeekerId)
                      .OnDelete(DeleteBehavior.Cascade);

            });
            /* =========================
               JobIndustry (Lookup)
            ========================== */
            builder.Entity<JobIndustry>(entity =>
            {
                entity.ToTable("JobIndustries");

                entity.HasKey(i => i.Id);

                entity.Property(i => i.IndustryName)
                      .IsRequired()
                      .HasMaxLength(150);
            });

            /* =========================
               EmploymentType (Lookup)
            ========================== */
            builder.Entity<EmploymentType>(entity =>
            {
                entity.ToTable("EmploymentTypes");

                entity.HasKey(et => et.Id);

                entity.Property(et => et.TypeName)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            /* =========================
               JobStatus (Lookup)
            ========================== */
            builder.Entity<JobStatus>(entity =>
            {
                entity.ToTable("JobStatuses");

                entity.HasKey(js => js.Id);

                entity.Property(js => js.StatusName)
                      .IsRequired()
                      .HasMaxLength(50);
            });

         

            /* =========================
               JobPost
            ========================== */
            builder.Entity<JobPost>(entity =>
            {
                entity.ToTable("JobPosts");

                entity.HasKey(j => j.Id);

                entity.Property(j => j.Title)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(j => j.Responsibilities)
                    .IsRequired()
                    .HasMaxLength(2000);
                entity.Property(j => j.Requirements)
                    .IsRequired()
                    .HasMaxLength(2000);
                entity.Property(j => j.EducationOrExperience)
                    .IsRequired()
                    .HasMaxLength(2000);
                entity.Property(j => j.Description)
                      .IsRequired()
                      .HasMaxLength(3000);

                entity.Property(j => j.Location)
                      .HasMaxLength(200);

                entity.Property(j => j.MinSalary)
                      .HasColumnType("decimal(18,2)");

                entity.Property(j => j.MaxSalary)
                      .HasColumnType("decimal(18,2)");

                entity.Property(j => j.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(j => j.EmployerProfile)
                      .WithMany(ep => ep.JobPosts)
                      .HasForeignKey(j => j.EmployerProfileId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(j => j.Industry)
                      .WithMany()
                      .HasForeignKey(j => j.IndustryId);

                entity.HasOne(j => j.EmploymentType)
                      .WithMany()
                      .HasForeignKey(j => j.EmploymentTypeId);

                entity.HasOne(j => j.Status)
                      .WithMany()
                      .HasForeignKey(j => j.StatusId);

                entity.HasOne(j => j.WorkMode)
                      .WithMany()
                      .HasForeignKey(j => j.WorkModeId);

                entity.HasOne(j => j.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(j => j.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            /* =========================
               JobApplication
            ========================== */
            builder.Entity<JobApplication>(entity =>
            {
                entity.ToTable("JobApplications");

                entity.HasKey(ja => ja.Id);

                entity.Property(ja => ja.CoverLetter)
                      .HasMaxLength(500);

                entity.Property(ja => ja.LinkedInUrl)
                      .HasMaxLength(500);
                entity.Property(ja => ja.ExperienceSummary)
                .HasMaxLength(500);
                entity.Property(ja => ja.Motivation)
               .HasMaxLength(500);
                entity.Property(ja => ja.Status)
      .HasConversion<int>()
      .IsRequired();

                entity.Property(ja => ja.AppliedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(ja => ja.JobSeeker)
                      .WithMany(js => js.Applications)
                      .HasForeignKey(ja => ja.JobSeekerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ja => ja.JobPost)
                      .WithMany(j => j.Applications)
                      .HasForeignKey(ja => ja.JobPostId)
                      .OnDelete(DeleteBehavior.Cascade);

             
            });

            /* =========================
               JobApplicationNote
            ========================== */
            builder.Entity<JobApplicationNote>(entity =>
            {
                entity.ToTable("JobApplicationNotes");

                entity.HasKey(n => n.Id);

                entity.Property(n => n.NoteText)
                      .IsRequired()
                      .HasMaxLength(2000);

                entity.Property(n => n.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(n => n.Application)
                      .WithMany(a => a.Notes)
                      .HasForeignKey(n => n.ApplicationId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(n => n.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(n => n.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<JobIndustry>().HasData(
    new JobIndustry { Id = 1, IndustryName = "IT & Software" },
    new JobIndustry { Id = 2, IndustryName = "Finance" },
    new JobIndustry { Id = 3, IndustryName = "Healthcare" },
    new JobIndustry { Id = 4, IndustryName = "Education" },
    new JobIndustry { Id = 5, IndustryName = "Marketing" },
    new JobIndustry { Id = 6, IndustryName = "Construction" },
    new JobIndustry { Id = 7, IndustryName = "Retail" },
    new JobIndustry { Id = 8, IndustryName = "Manufacturing" },
    new JobIndustry { Id = 9, IndustryName = "Transportation" },
    new JobIndustry { Id = 10, IndustryName = "Hospitality" }


   
);
            builder.Entity<EmploymentType>().HasData(
    new EmploymentType { Id = 1, TypeName = "Full-time" },
    new EmploymentType { Id = 2, TypeName = "Part-time" },
    new EmploymentType { Id = 3, TypeName = "Contract" },
    new EmploymentType { Id = 4, TypeName = "Internship" }
 
);
            builder.Entity<JobStatus>().HasData(
    new JobStatus { Id = 1, StatusName = "Draft" },
    new JobStatus { Id = 2, StatusName = "Published" },
    new JobStatus { Id = 3, StatusName = "Closed" }
);
            builder.Entity<WorkMode>().HasData(
       new WorkMode { Id = 1, Name = "OnSite" },
       new WorkMode { Id = 2, Name = "Remote" },
       new WorkMode { Id = 3, Name = "Hybrid" }
   );


        }

    }
}
