using Domain.Entities;
using Domain.Entities.LooksUp;
using Infrastructure.Implementation.AuthService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployerDashBoard : ControllerBase
    {
        private readonly AppDBContext _context;

        public EmployerDashBoard(AppDBContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetDashboard()
        //{
        //    var appUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(appUserId))
        //        return Unauthorized();

        //    // 2️⃣ Map AppUserId (string) → JobSeekerId (int)
        //    var employer = await _context.EmployerProfiles
        //                            .FirstOrDefaultAsync(e => e.UserId == appUserId);
        //    if (employer == null)
        //        return NotFound("JobSeeker profile not found.");

        //    int employerId = employer.Id;

        //    var today = DateTime.UtcNow.Date;
        //    var jobs = await _context.JobPosts.Include(j => j.Status)
        //   .Include(j => j.Applications)
        //      .Where(j =>
        //          j.CompanyId == employer.Company.Id).ToListAsync();




        //    var activeJobs = jobs.Count(j => j.Status.StatusName == "Published");

        //    // Expiring soon (next 7 days)
        //    var expiringSoon = jobs.Count(j => j.Status.StatusName == "Published" && j.CreatedAt.AddDays(30) <= today.AddDays(7));

        //    // Total applications
        //    var totalApplications = jobs.Sum(j => j.Applications.Count);

        //    // New today
        //    var newToday = jobs.Sum(j => j.Applications.Count(a => a.AppliedAt.Date == today));

        //    // Posted Jobs Table
        //    var postedJobs = jobs.Select(j => new
        //    {
        //        j.Id,
        //        j.Title,
        //        Status = j.Status.StatusName,
        //        Applications = j.Applications.Count,
        //        Posted = j.CreatedAt,
        //        ExpiryDate = j.CreatedAt.AddDays(30) // Example expiry logic
        //    }).ToList();

        //    // Recent Applications (latest 5)
        //    var recentApplications = await _context.JobApplications
        //        .Include(a => a.JobSeeker)
        //            .ThenInclude(js => js.User)
        //        .Include(a => a.JobPost)
        //        .Where(a => a.JobPost.EmployerProfileId == employer.Id)
        //        .OrderByDescending(a => a.AppliedAt)
        //        .Take(5)
        //        .Select(a => new
        //        {
        //            ApplicantName = a.JobSeeker.User.FullName,
        //            ApplicantImage = a.JobSeeker.ProfileImage.ImageUrl,
        //            JobTitle = a.JobPost.Title,
        //            Status = a.Status,
        //            a.AppliedAt
        //        }).ToListAsync();

        //    return Ok(new
        //    {
        //        ActiveJobs = activeJobs,
        //        ExpiringSoon = expiringSoon,
        //        TotalApplications = totalApplications,
        //        NewToday = newToday,
        //        PostedJobs = postedJobs,
        //        RecentApplications = recentApplications
        //    });


        //}
    }
}
