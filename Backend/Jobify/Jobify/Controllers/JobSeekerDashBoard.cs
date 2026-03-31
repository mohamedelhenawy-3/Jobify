using Application.Interfaces.JobApplication;
using Application.Interfaces.SavedJob;
using Domain.Entities.LooksUp;
using Infrastructure.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobSeekerDashBoard : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly ISavedJob _savedjobs;
        private readonly IJobApplication _jobapp;

        public JobSeekerDashBoard(AppDBContext context, ISavedJob savedjob,IJobApplication jobapp)
        {
            _context = context;
            _savedjobs = savedjob;
            _jobapp = jobapp;
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetDashboard()
        {
            var appUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(appUserId))
                return Unauthorized();

            var jobSeeker = _context.JobSeekers.FirstOrDefault(js => js.UserId == appUserId);
            if (jobSeeker == null)
                return NotFound("JobSeeker profile not found.");

            int jobSeekerId = jobSeeker.Id;

            var totalApplications = _context.JobApplications.Count(a => a.JobSeekerId == jobSeekerId);


            var underReviewApplications = _context.JobApplications
                                                 .Count(a => a.JobSeekerId == jobSeekerId && a.Status == ApplicationStatus.UnderReview);

            var interviewApplications = _context.JobApplications
                                               .Count(a => a.JobSeekerId == jobSeekerId && a.Status == ApplicationStatus.Interviewed);

            var appliedJobs = await _jobapp.GetMyApplications();
           var savedJobs = await _savedjobs.GetSavedJobsAsync();

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
