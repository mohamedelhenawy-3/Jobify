using Application.DTOs.Comapny.RefactorCompany;
using Application.DTOs.Employer;
using Application.DTOs.JobSeeker.JobSeekers;
using Application.DTOs.User;
using Application.Interfaces.PublicProfile;
using Infrastructure.Implementation.AuthService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.PublicProfiles
{
    public class PublicProfileService : IPublicProfileService
    {
        private readonly AppDBContext _context;
        private const int RecentJobsLimit = 5;

        public PublicProfileService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<PublicCompanyProfileDto?> GetCompanyProfileAsync(int companyId, int maxJobs = 10)
        {
            maxJobs = Math.Clamp(maxJobs, 1, 50);

            var company = await _context.Companies
                .AsNoTracking()
                .Where(x => x.Id == companyId && x.IsPublic)
                .Select(x => new PublicCompanyProfileDto
                {
                    Id = x.Id,
                    CompanyName = x.CompanyName,
                    Logo = x.Logo.ImageUrl,
                    Industry = x.Industry,
                    Location = x.Location,
                    Description = x.Description,
                    CompanySize = x.CompanySize,
                    Founded = x.Founded,
                    CompanyWebsite = x.CompanyWebsite,
                    IsVerified = x.IsVerified,
                    MembersCount = _context.EmployerProfiles
                        .Count(e => e.CompanyId == x.Id && e.IsActive)
                })
                .FirstOrDefaultAsync();

            if (company == null) return null;

            // open jobs: never show drafts or closed jobs
            // ADAPT: enable the status condition with your real "open" value from JobStatus
            var openJobs = _context.JobPosts
                .AsNoTracking()
                .Where(j => j.CompanyId == companyId /* && j.Status == JobStatus.Open */);

            company.OpenJobsCount = await openJobs.CountAsync();
            company.OpenJobs = await openJobs
                .OrderByDescending(j => j.CreatedAt)
                .Take(maxJobs)
                .Select(j => new PublicCompanyJobDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Location = j.Location,
                    CreatedAt = j.CreatedAt
                })
                .ToListAsync();

            return company;
        }

        private static void HideEmployerDetails(PublicEmployerProfileDto e)
        {
            e.LinkedInUrl = null;
            e.Role = "";
            e.JoinedCompanyAt = default;
            e.JobsPosted = 0;
            e.OpenJobsCount = 0;
            e.RecentJobs = new List<PublicJobSummaryDto>();
        }

        private static void HideSeekerDetails(PublicJobSeekerProfileDto s)
        {
            s.Skills = new List<string>();
            s.Experiences = new List<ExperienceDto>();
            s.Educations = new List<EducationDto>();
            s.GithubUrl = null;
            s.WebsiteUrl = null;
        }

        private async Task<PublicEmployerProfileDto?> GetEmployerAsync(string userId)
        {
            var employer = await _context.EmployerProfiles
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.IsActive)
                .Select(x => new PublicEmployerProfileDto
                {
                    IsPrivate = !x.IsPublic,
                    UserId = x.Id,
                    EmployerId = x.Id,
                    FullName = x.User.FullName,
                    ImageUrl = x.ProfileImage.ImageUrl,
                    LinkedInUrl = x.User.LinkedInUrl,
                    Bio = x.Bio,
                    Position = x.Position,
                    Role = x.Role.ToString(),
                    JoinedCompanyAt = x.JoinedCompanyAt,
                    JobsPosted = _context.JobPosts.Count(j => j.PostedByEmployerId == x.Id),

                    CompanyId = _context.Companies
                        .Where(c => c.Id == x.CompanyId && c.IsPublic)
                        .Select(c => (int?)c.Id).FirstOrDefault(),
                    CompanyName = _context.Companies
                        .Where(c => c.Id == x.CompanyId && c.IsPublic)
                        .Select(c => c.CompanyName).FirstOrDefault(),
                    CompanyLogo = _context.Companies
                        .Where(c => c.Id == x.CompanyId && c.IsPublic)
                        .Select(c => c.Logo.ImageUrl).FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (employer == null) return null;

            if (employer.IsPrivate)
            {
                HideEmployerDetails(employer);
                return employer;
            }

            var openJobs = _context.JobPosts
                .AsNoTracking()
                .Where(j => j.PostedByEmployerId == employer.EmployerId);

            employer.OpenJobsCount = await openJobs.CountAsync();
            employer.RecentJobs = await openJobs
                .OrderByDescending(j => j.CreatedAt)
                .Take(RecentJobsLimit)
                .Select(j => new PublicJobSummaryDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Location = j.Location,
                    CreatedAt = j.CreatedAt
                })
                .ToListAsync();

            return employer;
        }

        private async Task<PublicJobSeekerProfileDto?> GetJobSeekerAsync(string userId)
        {
            var seeker = await _context.JobSeekers
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => new PublicJobSeekerProfileDto
                {
                    IsPrivate = !x.IsPublic,
                    UserId = x.Id,
                    SeekerId = x.Id,
                    FullName = x.User.FullName,
                    ImageUrl = x.ProfileImage.ImageUrl,
                    Headline = x.Headline,
                    Location = x.User.Location,
                    About = x.About,
                    GithubUrl = x.GithubUrl,
                    WebsiteUrl = x.WebsiteUrl,
                    IsOpenToWork = x.IsOpenToWork,
                    Skills = x.Skills.Select(s => s.Name).ToList(),
                    Experiences = x.Experiences
                        .OrderByDescending(e => e.EndDate == null)
                        .ThenByDescending(e => e.StartDate)
                        .Select(e => new ExperienceDto
                        {
                            Title = e.Title,
                            Company = e.Company,
                            Location = e.Location,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate,
                            IsCurrent = e.EndDate == null,
                            Description = e.Description
                        }).ToList(),
                    Educations = x.Educations
                        .OrderByDescending(e => e.EndDate == null)
                        .ThenByDescending(e => e.StartDate)
                        .Select(e => new EducationDto
                        {
                            School = e.School,
                            Degree = e.Degree,
                            FieldOfStudy = e.FieldOfStudy,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate,
                            Description = e.Description
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (seeker != null && seeker.IsPrivate)
                HideSeekerDetails(seeker);

            return seeker;
        }

        public async Task<PublicUserProfileDto?> GetUserProfileAsync(string userId)
        {
            var employer = await GetEmployerAsync(userId);
            if (employer != null) return employer;

            return await GetJobSeekerAsync(userId);
        }



        /// <summary>
        /// ///////////////////////////
        /// </summary>
        /// <param name="employerId"></param>
        /// <returns></returns>
        public Task<PublicEmployerProfileDto?> GetEmployerByIdAsync(string employerId)
            => GetEmployerAsync(employerId);

        public Task<PublicJobSeekerProfileDto?> GetJobSeekerByIdAsync(string seekerId)
            => GetJobSeekerAsync(seekerId);
    }
}
