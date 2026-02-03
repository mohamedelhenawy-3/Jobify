using Application.DTOs;
using Application.DTOs.JobApplication;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.JobApplication;
using Domain.Entities;
using Infrastructure.DBContext;
using Infrastructure.Implementation.CLoudinary;
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

        public JobApplicationService(
              AppDBContext context,
              CloudinaryService cloudinaryService,
              IHttpContextAccessor httpContextAccessor)
        {
            _db = context;
            _cloudinaryService = cloudinaryService;
            _httpaccessor = httpContextAccessor;
        }
        private string? GetCurrentUserId()
        {
            var user = _httpaccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async  Task<SuccessResponse> CreateApplication(CreateJobApplication jobdto, int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (profile == null)
                throw new Exception("JobSeeker profile not found");
            var job = _db.JobPosts.Where(x => x.Id == id).Select(c => c.Id).FirstOrDefault();

            var jobapplication = new JobApplication()
            {
                CoverLetter = jobdto.CoverLetter,
                LinkedInUrl = jobdto.LinkedInUrl,
                Motivation = jobdto.Motivation,
                ExperienceSummary = jobdto.ExperienceSummary
            };
            jobapplication.JobPostId = job;
            jobapplication.JobSeekerId = profile.Id;

            await _db.AddAsync(jobapplication);
            await _db.SaveChangesAsync();
            return new SuccessResponse
            {

                Message = "Application Added successfully"
            };

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
                            ProfileImageUrl = a.JobSeeker.ProfileImage.ImageUrl
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
                    ResumeUrl = a.JobSeeker.Resume != null ? a.JobSeeker.Resume.PdfUrl : null
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

    }
}
