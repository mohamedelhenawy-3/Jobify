using Application.DTOs;
using Application.DTOs.JobApplication;
using Application.DTOs.SuccefullOp;
using Application.Events;
using Application.Interfaces.JobApplication;
using CloudinaryDotNet;
using Domain.Entities;
using Domain.Entities.Event;
using Domain.Entities.LooksUp;
using Infrastructure.DBContext;
using Infrastructure.Implementation.CLoudinary;
using Infrastructure.Implementation.FileUploadService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
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
                
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("Unauthorized");

                var profile = await _db.JobSeekers
                    .FirstOrDefaultAsync(js => js.UserId == userId);

                if (profile == null)
                    throw new Exception("JobSeeker profile not found");

               
                var job = await _db.JobPosts
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (job == null)
                    throw new Exception("Job not found");

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

                var jobapplication = new JobApplication
                {
                    CoverLetter = jobdto?.CoverLetter,
                    LinkedInUrl = jobdto?.LinkedInUrl,
                    Motivation = jobdto?.Motivation,
                    ExperienceSummary = jobdto?.ExperienceSummary,
                    ResumeUrl = url,
                    JobPostId = job.Id,
                    JobSeekerId = profile.Id,
                    Status = ApplicationStatus.Applied, 
                    AppliedAt = DateTime.UtcNow
                };

            
                await _db.JobApplications.AddAsync(jobapplication);
                await _db.SaveChangesAsync();

                return new SuccessResponse
                {
                    Message = "Application added successfully"
                   
                };
            }
            catch (Exception ex)
            {
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
                    .ThenInclude(x => x.Company)      
                        .ThenInclude(c => c.Logo)      
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
                    CompanyName = a.JobPost.Company.CompanyName,  
                    Logo = a.JobPost.Company.Logo.ImageUrl, 
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
                          .Include(e => e.Company)          
                          .FirstOrDefaultAsync(js => js.UserId == userId);
            if (profile == null)
                throw new Exception("Employer profile not found");

            if (profile.Company == null)
                throw new Exception("You don't have a company yet");

            var application = await _db.JobApplications
      .Where(a => a.JobPost.CompanyId == profile.Company.Id)
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
          ResumeUrl = a.ResumeUrl
      }).ToListAsync();
            return application;
        }

        public async Task<AppliedJoSeekerDetails> GetAppliedJobSeekerDeyails(int applicationId)
        {
            var applicationDetails = await _db.JobApplications
                .Where(a => a.Id == applicationId)
                .Include(a => a.JobSeeker).ThenInclude(js => js.User)
                .Include(a => a.JobSeeker)
                .Include(a => a.JobPost)
                .Select(a => new AppliedJoSeekerDetails
                {
                 
                    CoverLetter = a.CoverLetter,
                    Motivation = a.Motivation,
                    ExperienceSummary = a.ExperienceSummary,
                    LinkedInUrl = a.LinkedInUrl,
                    ResumeUrl = a.ResumeUrl

                }).FirstOrDefaultAsync();
            Console.WriteLine(applicationDetails.ResumeUrl);

            return applicationDetails!;
        }
        public async Task<SuccessResponse> UpdateJobStatus(UpdateAppliedJobStatus updatestatusdto, int applicationId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.EmployerProfiles
                .Include(e => e.Company)            
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (profile == null)
                throw new Exception("Employer profile not found");

            if (profile.Company == null)
                throw new Exception("You don't have a company yet");

            var application = await _db.JobApplications
                .FirstOrDefaultAsync(a =>
                    a.Id == applicationId &&
                    a.JobPost.CompanyId == profile.Company.Id); 

            if (application == null)
                throw new Exception("Application not found or you are not authorized");

            application.Status = updatestatusdto.status;
            if(application.Status == ApplicationStatus.Interviewed)
            {
                if (updatestatusdto.InterviewDate == null || string.IsNullOrWhiteSpace(updatestatusdto.InterviewLocation))
                    throw new Exception("Interview data is required");
                var B = new Interview
                {
                    JobApplicationId=application.Id,
                    InterviewDate = updatestatusdto.InterviewDate.Value,
                    Location = updatestatusdto.InterviewLocation!,
                    Notes = updatestatusdto.Notes
                };

                _db.Interviews.Add(B);

            }
            await _db.SaveChangesAsync();
            var interview = await _db.Interviews
      .FirstOrDefaultAsync(i => i.JobApplicationId == application.Id);

            await _Notify.Notify(new ApplicationStatusEvent
            {
                Application = application,
                Interview = interview
            });
            return new SuccessResponse
            {
                Message = "Application status updated successfully"
            };
        }

        public async Task<IEnumerable<MyApplicationDto>> GetMyApplications(ApplicationFilterDto filterdto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (profile == null)
                throw new Exception("JobSeeker profile not found");

            var query = _db.JobApplications
                     .Include(a => a.JobPost)
                        .ThenInclude(j => j.Company)
                                .Where(a => a.JobSeekerId == profile.Id)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterdto.Status) && filterdto.Status.ToLower() != "all")
            {
                if (Enum.TryParse<ApplicationStatus>(filterdto.Status, true, out var statusEnum))
                {
                    query = query.Where(a => a.Status == statusEnum);
                }
            }
            if (!string.IsNullOrWhiteSpace(filterdto.Search))
            {
                string term = filterdto.Search.ToLower();
                query = query.Where(a =>
                    a.JobPost.Title.ToLower().Contains(term) ||
                    a.JobPost.Company.CompanyName.ToLower().Contains(term)
                );
            }

            return await query
        .OrderByDescending(a => a.AppliedAt)
        .Select(a => new MyApplicationDto
        {
            ApplicationId = a.Id,
            Status = a.Status.ToString(),
            AppliedAt = a.AppliedAt,
            JobTitle = a.JobPost.Title,
            CompanyName = a.JobPost.Company.CompanyName,
            Logo = a.JobPost.Company.Logo.ImageUrl,
            JobLocation = a.JobPost.Location,
            WorkMode = a.JobPost.WorkMode.Name,
            EmploymentType = a.JobPost.EmploymentType.TypeName
        })
        .ToListAsync();
        }

        public async Task<IEnumerable<AppliedJobSeekerDto>> appliedapplications(ApplicationFilterDto filterdto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.EmployerProfiles.Include(x=>x.Company)
                          .FirstOrDefaultAsync(js => js.UserId == userId);
            if (profile == null)
                throw new Exception("Employer profile not found");

            if (profile.Company == null)
                throw new Exception("You don't have a company yet");

            var query = _db.JobApplications.Where(a => a.JobPost.CompanyId == profile.Company.Id)
                 .Include(x => x.JobPost)
                 .Include(x=>x.JobSeeker).ThenInclude(x=>x.ProfileImage)
                 .Include(a => a.JobSeeker)
                 .ThenInclude(js => js.User)
                
                 .AsQueryable();

            // Reuse the same status logic
            if (!string.IsNullOrWhiteSpace(filterdto.Status) && filterdto.Status.ToLower() != "all")
            {
                if (Enum.TryParse<ApplicationStatus>(filterdto.Status, true, out var statusEnum))
                {
                    query = query.Where(a => a.Status == statusEnum);
                }
            }

            if (!string.IsNullOrWhiteSpace(filterdto.Search))
            {
                string term = filterdto.Search.ToLower();
                query = query.Where(a => a.JobSeeker.User.FullName.ToLower().Contains(term)
                || a.JobSeeker.User.Email.ToLower().Contains(term) || a.JobPost.Title.ToLower().Contains(term)
                || a.JobPost.Company.CompanyName.ToLower().Contains(term));
            }
            return await query
      .OrderByDescending(a => a.AppliedAt)
      .Select(a => new AppliedJobSeekerDto
      {
          ApplicationId = a.Id,
          Status = a.Status.ToString(),
          AppliedAt = a.AppliedAt,
          JobTitle = a.JobPost.Title,
          JobSeekerEmail = a.JobSeeker.User.Email,
          JobSeekerFullName = a.JobSeeker.User.UserName,
          ProfileImageUrl = a.JobSeeker.ProfileImage.ImageUrl,
          ResumeUrl = a.ResumeUrl
      })
      .ToListAsync();
        }
    }
}
