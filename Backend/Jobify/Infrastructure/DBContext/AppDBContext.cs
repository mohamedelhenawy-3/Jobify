using Application.DTOs.JobSeeker.JobSeekers;
using Domain.Entities;
using Domain.Entities.Comp;
using Domain.Entities.LooksUp;
using Domain.Entities.LooksUp.JobEnum;
using Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.AuthService
{
    public class AppDBContext :IdentityDbContext<AppUser>
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<CompanyJoinRequest> CompanyJoinRequests { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<SavedJob> SavedJobs { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<JobSeeker> JobSeekers { get; set; }
        public DbSet<EmployerProfile> EmployerProfiles { get; set; }
        public DbSet<ProfileImage> ProfileImages { get; set; }
        public DbSet<Pdf> Pdfs { get; set; }
        public DbSet<SavedJobAlert> SavedJobAlerts { get; set; }

        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<JobApplicationNote> JobApplicationNotes { get; set; }
        public DbSet<JobIndustry> JobIndustries { get; set; }
        public DbSet<SeekerLanguage> Languages { get; set; }
        public DbSet<SeekerCertificate> Certificates { get; set; } 
        public DbSet<SeekerProject> Projects { get; set; } 
        public DbSet<WorkMode> WorkModes { get; set; }
        public DbSet<EmploymentType> EmploymentTypes { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<CompanyActivity> CompanyActivities { get; set; }
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
                entity.Property(ep => ep.PhoneNumber)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(ep => ep.Location)
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
            builder.Entity<CompanyActivity>(b =>
            {
                b.Property(x => x.Type).HasConversion<string>().HasMaxLength(40);
                b.Property(x => x.ActorName).HasMaxLength(150);
                b.Property(x => x.TargetName).HasMaxLength(150);
                b.Property(x => x.Message).HasMaxLength(500);
                b.HasIndex(a => new { a.CompanyId, a.CreatedAt });
                b.HasIndex(a => new { a.ActorEmployerId, a.CreatedAt });
                b.HasIndex(a => new { a.TargetEmployerId, a.CreatedAt });
                b.HasIndex(x => new { x.CompanyId, x.CreatedAt });
            });
            builder.Entity<SeekerLanguage>()
             .HasOne<JobSeeker>().WithMany(j => j.Languages)
             .HasForeignKey(x => x.JobSeekerProfileId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SeekerCertificate>()
                .HasOne<JobSeeker>().WithMany(j => j.Certificates)
                .HasForeignKey(x => x.JobSeekerProfileId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<SeekerProject>(entity =>
            {
                entity.ToTable("SeekerProjects");

                entity.Property(x => x.Title).IsRequired().HasMaxLength(150);
                entity.Property(x => x.Description).HasMaxLength(2000);
                entity.Property(x => x.Url).HasMaxLength(500);
                entity.Property(x => x.Technologies).HasMaxLength(500);
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(x => x.JobSeekerProfileId);

                entity.HasOne<JobSeeker>()
                      .WithMany(j => j.Projects)
                      .HasForeignKey(x => x.JobSeekerProfileId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.Images)
                      .WithOne(i => i.SeekerProject)
                      .HasForeignKey(i => i.SeekerProjectId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            /* =========================
               JobSeeker
            ========================== */
            builder.Entity<JobSeeker>(entity =>
            {
                entity.ToTable("JobSeekers");

                entity.HasKey(js => js.Id);



                entity.HasIndex(p => p.Username).IsUnique().HasFilter("[Username] IS NOT NULL");
                entity.HasMany(p => p.Experiences).WithOne().HasForeignKey(x => x.SeekerProfileId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(p => p.Educations).WithOne().HasForeignKey(x => x.SeekerProfileId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(p => p.Skills).WithOne().HasForeignKey(x => x.SeekerProfileId).OnDelete(DeleteBehavior.Cascade);

                entity.Property(js => js.ResumeUrl)
                      .HasMaxLength(500);

           

                entity.HasOne(js => js.User)
                      .WithOne(u => u.JobSeeker)
                      .HasForeignKey<JobSeeker>(js => js.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(js => js.Resume) // one-to-one Resume
                      .WithOne(p => p.JobSeeker)
                      .HasForeignKey<Pdf>(p => p.JobSeekerId) // ✅ FK is on Pdf
                      .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<Skill>().HasIndex(s => new { s.SeekerProfileId, s.Name }).IsUnique();

            /* =========================
               EmployerProfile
            ========================== */
            builder.Entity<EmployerProfile>(entity =>
            {
                entity.ToTable("EmployerProfiles");

                entity.HasKey(ep => ep.Id);

                entity.Property(ep => ep.Position)

                      .HasMaxLength(200);


                entity.Property(ep => ep.Permissions)
         .HasConversion(
             v => string.Join(',', v),
             v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
         .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
             (c1, c2) => c1!.SequenceEqual(c2!),
             c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
             c => c.ToList()));
                entity.Property(ep => ep.Bio)
                   
                     .HasMaxLength(200);
                entity.HasOne(ep => ep.ProfileImage)
                  .WithOne(pi => pi.EmployerProfile)
                    .HasForeignKey<ProfileImage>(pi => pi.EmployerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ep => ep.Company)
                          .WithMany(c => c.EmployerProfiles)
                         .HasForeignKey(ep => ep.CompanyId)
                         .OnDelete(DeleteBehavior.SetNull);


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

                entity.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(500);
                entity.Property(pi => pi.PublicId).HasMaxLength(300);
                entity.Property(pi => pi.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(pi => new { pi.SeekerProjectId, pi.DisplayOrder });

                entity.HasOne(pi => pi.JobSeeker)
                      .WithOne(js => js.ProfileImage)
                      .HasForeignKey<ProfileImage>(pi => pi.JobSeekerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pi => pi.EmployerProfile)
                      .WithOne(ep => ep.ProfileImage)
                      .HasForeignKey<ProfileImage>(pi => pi.EmployerProfileId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pi => pi.Company)
                      .WithOne(c => c.Logo)
                      .HasForeignKey<ProfileImage>(pi => pi.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Company>(entity =>
            {
                entity.ToTable("Companies");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.CompanyName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.CompanyWebsite)
                    .HasMaxLength(200);

                entity.Property(c => c.Description)
                    .HasMaxLength(2000);

                entity.Property(c => c.Location)
                    .HasMaxLength(100);

                entity.Property(c => c.Industry)
                    .HasMaxLength(100);

                entity.Property(c => c.CompanySize)
                    .HasMaxLength(50);

                entity.Property(c => c.Founded)
                    .HasMaxLength(4);

                entity.Property(c => c.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(c => c.IsVerified).HasDefaultValue(false);
                entity.Property(c => c.VerificationStatus)
                      .HasConversion<int>()
                      .HasDefaultValue(CompanyVerificationStatus.Pending);
                entity.Property(c => c.RejectionReason).HasMaxLength(1000);
                entity.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(c => c.VerificationSubmittedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(c => c.CreatedByEmployer)
                      .WithMany()
                      .HasForeignKey(c => c.CreatedByEmployerId)
                      .OnDelete(DeleteBehavior.Restrict);
                // Company -> Jobs
                entity.HasMany(c => c.JobPosts)
                    .WithOne(j => j.Company)
                    .HasForeignKey(j => j.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Company -> Logo
                entity.HasOne(c => c.Logo)
                    .WithOne(p => p.Company)
                    .HasForeignKey<ProfileImage>(p => p.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

         

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
     
    
            builder.Entity<JobPost>(entity =>
            {
                entity.HasKey(j => j.Id);

                entity.Property(j => j.SalaryMin)
                      .HasPrecision(18, 2);

                entity.Property(j => j.SalaryMax)
                      .HasPrecision(18, 2);

                entity.Property(j => j.JobType)
                      .HasConversion<string>();

                entity.Property(j => j.ExperienceLevel)
                      .HasConversion<string>();

               
                entity.HasOne(j => j.Company)
                      .WithMany(c => c.JobPosts)
                      .HasForeignKey(j => j.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(j => j.PostedByEmployer)
                      .WithMany()
                      .HasForeignKey(j => j.PostedByEmployerId)
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
                      .WithMany(j => j.JobApplications)
                      .HasForeignKey(ja => ja.JobPostId)
                      .OnDelete(DeleteBehavior.Cascade);

             
            });
            builder.Entity<SavedJobAlert>()
              .HasIndex(a => new { a.UserId, a.Keyword })
              .IsUnique();

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


        }

    }
}
