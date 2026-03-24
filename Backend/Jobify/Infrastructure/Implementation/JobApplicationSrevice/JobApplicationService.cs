using Application.DTOs;
using Application.DTOs.JobApplication;
using Application.DTOs.SuccefullOp;
using Application.Events;
using Application.Interfaces.JobApplication;
using CloudinaryDotNet;
using Domain.Entities;
using Domain.Entities.LooksUp;
using Infrastructure.DBContext;
using Infrastructure.Implementation.CLoudinary;
using Infrastructure.Implementation.FileUploadService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure.Implementation.JobApplicationSrevice
{
    public class JobApplicationService : IJobApplication
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpaccessor;
        private readonly CloudinaryService _cloudinaryService;
        private readonly SupabaseService _supabase;
        public readonly ApplicationStatusNotifier _Notify;

        public JobApplicationService(
              AppDBContext context,
              ApplicationStatusNotifier Notify,
              CloudinaryService cloudinaryService,
              IHttpContextAccessor httpContextAccessor, SupabaseService supabase
              )
        {
            _db = context;
            _cloudinaryService = cloudinaryService;
            _httpaccessor = httpContextAccessor;
            _supabase = supabase;
            _Notify = Notify;
        }
        private string? GetCurrentUserId()
        {
            var user = _httpaccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<SuccessResponse> CreateApplication(CreateJobApplication jobdto, int id)
        {
            try
            {
                // 🔹 1. Validate user
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("Unauthorized");

                // 🔹 2. Get JobSeeker profile
                var profile = await _db.JobSeekers
                    .FirstOrDefaultAsync(js => js.UserId == userId);

                if (profile == null)
                    throw new Exception("JobSeeker profile not found");

                // 🔹 3. Validate Job
                var job = await _db.JobPosts
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (job == null)
                    throw new Exception("Job not found");

                // 🔹 4. Handle Resume Upload (SAFE)
                string? url = null;

                if (jobdto != null && jobdto.Resume != null)
                {
                    try
                    {
                        url = await _supabase.UploadResume(jobdto.Resume);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Upload Error: " + ex.Message);
                        throw new Exception("Resume upload failed");
                    }
                }

                // 🔹 5. Create Application (ONLY IF ALL OK)
                var jobapplication = new JobApplication
                {
                    CoverLetter = jobdto?.CoverLetter,
                    LinkedInUrl = jobdto?.LinkedInUrl,
                    Motivation = jobdto?.Motivation,
                    ExperienceSummary = jobdto?.ExperienceSummary,
                    ResumeUrl = url,
                    JobPostId = job.Id,
                    JobSeekerId = profile.Id,
                    Status = ApplicationStatus.Applied, // ✅ default
                    AppliedAt = DateTime.UtcNow
                };

                // 🔹 6. Save
                await _db.JobApplications.AddAsync(jobapplication);
                await _db.SaveChangesAsync();

                return new SuccessResponse
                {
                    Message = "Application added successfully"
                   
                };
            }
            catch (Exception ex)
            {
                // 🔥 IMPORTANT for debugging
                Console.WriteLine("ERROR: " + ex.ToString());

                return new SuccessResponse
                {
                    Message = ex.Message
                };
            }
        }
        public async Task<IEnumerable<MyApplications>> GetMyApplications()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (profile == null)
                throw new Exception("JobSeeker profile not found");
            var applications = await _db.JobApplications
                .Include(x => x.JobPost)
                   .ThenInclude(x => x.EmployerProfile)
                .Include(x => x.JobPost.Industry)
                .Include(x => x.JobPost.WorkMode)
                .Include(x => x.JobPost.EmploymentType)
                .Where(x => x.JobSeekerId == profile.Id)
                .Select(a => new MyApplications
                {
                    ApplicationId = a.Id,
                    Status = a.Status.ToString(),
                    AppliedAt = a.AppliedAt,

                    JobId = a.JobPost.Id,
                    JobTitle = a.JobPost.Title,
                    CompanyName = a.JobPost.EmployerProfile.CompanyName,
                    JobLocation = a.JobPost.Location,
                    WorkMode = a.JobPost.WorkMode.Name,
                    EmploymentType = a.JobPost.EmploymentType.TypeName,
                    CoverLetterUrl = a.CoverLetter
                }).ToListAsync();


            return applications;

        }

        public async  Task<IEnumerable<AppliedJobSeekers>> GetAppliedJobSeeker()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.EmployerProfiles
                .FirstOrDefaultAsync(js => js.UserId == userId);

            var application = await _db.JobApplications
               .Where(a => a.JobPost.EmployerProfileId == profile.Id)
                    .Include(a => a.JobPost)
                    .Include(a => a.JobSeeker)
                        .ThenInclude(js => js.User)
                    .Include(x => x.JobSeeker)
                        .ThenInclude(x => x.ProfileImage)

                        .Select(a => new AppliedJobSeekers
                        {
                            ApplicationId = a.Id,
                            Status = a.Status.ToString(),
                            AppliedAt = a.AppliedAt,
                            JobTitle = a.JobPost.Title,
                            JoSeekerEmail = a.JobSeeker.User.Email,
                            JoSeekerFullName = a.JobSeeker.User.UserName,
                            ProfileImageUrl = a.JobSeeker.ProfileImage.ImageUrl,
                            ResumeUrl =a.ResumeUrl
                        }).ToListAsync();

            return application;
        }

        public async Task<AppliedJoSeekerDetails> GetAppliedJobSeekerDeyails(int applicationId)
        {
            var applicationDetails = await _db.JobApplications
                .Where(a => a.Id == applicationId)
                .Include(a => a.JobSeeker).ThenInclude(js => js.User)
                .Include(a => a.JobSeeker).ThenInclude(js => js.Resume)
                .Include(a => a.JobPost)
                .Select(a => new AppliedJoSeekerDetails
                {
                 
                    CoverLetter = a.CoverLetter,
                    Motivation = a.Motivation,
                    ExperienceSummary = a.ExperienceSummary,
                    LinkedInUrl = a.LinkedInUrl,
                    ResumeUrl = a.ResumeUrl != null ? a.JobSeeker.ResumeUrl : null

                }).FirstOrDefaultAsync();

            return applicationDetails!;
        }
        public async Task<SuccessResponse> UpdateJobStatus(UpdateAppliedJobStatus updatestatusdto, int applicationId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.EmployerProfiles
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (profile == null)
                throw new Exception("Employer profile not found");

            var application = await _db.JobApplications
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPost.EmployerProfileId == profile.Id);

            if (application == null)
                throw new Exception("Application not found or you are not authorized");

            application.Status = updatestatusdto.status;

            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "Application status updated successfully"
            };
        }

        public async  Task<SuccessResponse> UpdateJobInterviewStatus(InterviewStatus updatestatusdto, int applicationId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.EmployerProfiles
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (profile == null)
                throw new Exception("Employer profile not found");

            var application = await _db.JobApplications
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPost.EmployerProfileId == profile.Id);

            if (application == null)
                throw new Exception("Application not found or you are not authorized");



            if (updatestatusdto.status != ApplicationStatus.Interviewed)
                throw new Exception("This endpoint is only for Interview status");

            if (updatestatusdto.InterviewDate == default)
                throw new Exception("Interview date is required");

            application.Status = ApplicationStatus.Interviewed;
            application.InterviewDate = updatestatusdto.InterviewDate;

            await _db.SaveChangesAsync();
            _Notify.Notify(application);


            return new SuccessResponse
            {
                Message = "Interview scheduled successfully"
            };
        }
    }
}
