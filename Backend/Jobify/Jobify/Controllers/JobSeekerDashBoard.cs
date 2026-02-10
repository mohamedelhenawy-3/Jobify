using Domain.Entities.LooksUp;
using Infrastructure.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobSeekerDashBoard : ControllerBase
    {
        private readonly AppDBContext _context;

        public JobSeekerDashBoard(AppDBContext context)
        {
            _context = context;
        }
        [HttpGet("me")]
        public IActionResult GetDashboard()
        {
            // 1️⃣ Get logged-in userId (string)
            var appUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(appUserId))
                return Unauthorized();

            // 2️⃣ Map AppUserId (string) → JobSeekerId (int)
            var jobSeeker = _context.JobSeekers.FirstOrDefault(js => js.UserId == appUserId);
            if (jobSeeker == null)
                return NotFound("JobSeeker profile not found.");

            int jobSeekerId = jobSeeker.Id;

            // 3️⃣ Now use jobSeekerId for queries
            var totalApplications = _context.JobApplications.Count(a => a.JobSeekerId == jobSeekerId);


            var underReviewApplications = _context.JobApplications
                                                 .Count(a => a.JobSeekerId == jobSeekerId && a.Status == ApplicationStatus.UnderReview);

            var interviewApplications = _context.JobApplications
                                               .Count(a => a.JobSeekerId == jobSeekerId && a.Status == ApplicationStatus.Interviewed);

            var appliedJobs = _context.JobApplications
                                      .Where(a => a.JobSeekerId == jobSeekerId)
                                      .Select(a => new {
                                          JobTitle = a.JobPost.Title,
                                          Company = a.JobPost.EmployerProfile.CompanyName,
                                          Location = a.JobPost.Location,
                                          DateApplied = a.AppliedAt,
                                          Status = a.Status
                                      }).ToList();

            var savedJobs = _context.SavedJobs
                                    .Where(s => s.UserId == appUserId)   // SavedJobs.UserId is string AppUserId
                                    .Select(s => new {
                                        s.Job.Title,
                                        s.Job.Location,
                                        s.Job.CreatedAt
                                    }).ToList();

            return Ok(new
            {
                TotalApplications = totalApplications,
                UnderReviewApplications = underReviewApplications,
                InterviewApplications = interviewApplications,
                AppliedJobs = appliedJobs,
                SavedJobs = savedJobs
            });
        }

    }
}
