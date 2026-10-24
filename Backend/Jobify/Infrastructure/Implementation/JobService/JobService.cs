using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.industry;
using Application.DTOs.JobPost;
using Application.DTOs.JobPost.JobDTO;
using Application.DTOs.JobPost.JobDTO.RecordsDTO;
using Application.DTOs.Pagenation;
using Application.DTOs.RecommendedJobbs;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.Job;
using Application.Interfaces.Logger;
using Application.Interfaces.Permisions;
using Application.SearchFeature.Dtos;
using Domain.Entities;
using Domain.Entities.Comp;
using Domain.Entities.LooksUp;
using Domain.Entities.LooksUp.EmployerPermisions;
using Domain.Entities.LooksUp.JobEnum;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace Infrastructure.Implementation.JobService
{
    public class JobService : IJobService
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpaccessor;
        private readonly IEmployerPermissionService _Ipermision;
        private readonly IActivityLogger _activity;

        private readonly ICurrentUser _currentuser;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(2);
        private readonly IMemoryCache _cache;

        public JobService(AppDBContext db, IMemoryCache cache,
            ICurrentUser currentuser ,IHttpContextAccessor http, IEmployerPermissionService Ipermision, IActivityLogger activity)
        {
            _db = db;
            _httpaccessor = http;
            _Ipermision = Ipermision;
             _activity=activity;
            _currentuser = currentuser;
            _cache = cache;

        }

        private string? GetCurrentUserId()
        {
            var user = _httpaccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // ───────────────────────── EMPLOYER: MY JOBS ─────────────────────────
        public async Task<List<EmployerJobItemDto>> GetMyJobsAsync()
        {
            var userId = GetCurrentUserId();
            var me = await _db.EmployerProfiles
                     .AsNoTracking()
                     .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
            if (me == null || me.CompanyId == null)
                return new List<EmployerJobItemDto>();

            var query = _db.JobPosts
                .AsNoTracking()
                .Where(j => j.CompanyId == me.CompanyId);

            bool seesWholeCompany = me.Role == EmployerCompanyRole.Owner || me.Role == EmployerCompanyRole.Admin;

            if (!seesWholeCompany)
                query = query.Where(j => j.PostedByEmployerId == me.Id);

            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Take(100)
                .Select(j => new EmployerJobItemDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Location = j.Location,
                    JobType = j.JobType.ToString(),
                    ExperienceLevel = j.ExperienceLevel.ToString(),
                    CreatedAt = j.CreatedAt,
                    IsActive = j.IsActive,
                    ApplicantsCount = j.JobApplications.Count(),
                    PostedBy = seesWholeCompany && j.PostedByEmployer != null && j.PostedByEmployer.User != null
                        ? j.PostedByEmployer.User.FullName
                        : null
                })
                .ToListAsync();

            return jobs;
        }

        public async Task<IEnumerable<ReturnedJobDto>?> GetEmployerJobsAsync()
        {
            var employer = await _Ipermision.GetCurrentEmployerWithPermissionAsync(EmployerPermissions.PostJobs);
            if (employer == null || employer.CompanyId == null)
                return null;

            var canSeeAllCompanyJobs = _Ipermision.HasPermission(employer, EmployerPermissions.ManageEmployerRoles);

            var query = _db.JobPosts
                .Where(j => j.CompanyId == employer.CompanyId);

            if (!canSeeAllCompanyJobs)
                query = query.Where(j => j.PostedByEmployerId == employer.Id);

            // 1. Database part only
            var rawJobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    LogoUrl = j.Company.Logo != null ? j.Company.Logo.ImageUrl : null,
                    j.Description,
                    j.Location,
                    j.SalaryMin,
                    j.SalaryMax,
                    JobTypeStr = j.JobType.ToString(),
                    ExperienceLevelStr = j.ExperienceLevel.ToString(),
                    j.IsActive,
                    j.CreatedAt,
                    j.Responsibilities,
                    j.Requirements,
                    CompanyName = j.Company.CompanyName,
                    CompanyCreatedAt = j.Company.CreatedAt,
                    ApplicationsCount = j.JobApplications.Count(),
                    PostedByUserId = j.PostedByEmployer != null ? j.PostedByEmployer.UserId : null,
                    PostedByName = j.PostedByEmployer != null && j.PostedByEmployer.User != null
                        ? j.PostedByEmployer.User.FullName
                        : null
                })
                .ToListAsync();

            var now = DateTime.UtcNow;

            return rawJobs.Select(j => new ReturnedJobDto
            {
                Id = j.Id,
                Title = j.Title,
                Logo = j.LogoUrl,
                Description = j.Description,
                Location = j.Location,
                MinSalary = j.SalaryMin,
                MaxSalary = j.SalaryMax,
                EmploymentType = j.JobTypeStr,
                ExprianceLevel = j.ExperienceLevelStr,
                Status = j.IsActive ? "Active" : "Closed",
                CreatedAt = j.CreatedAt,
                PostedAge = CalculateAgeText(j.CreatedAt, now),
                Requirements = j.Requirements,
                Responsibilities = j.Responsibilities,
                CompanyName = j.CompanyName,
                CompanyAge = CalculateAgeText(j.CompanyCreatedAt, now),
                TotalApplications = j.ApplicationsCount,
                PostedByEmployerId = j.PostedByUserId,
                PostedByName = j.PostedByName ?? "Company Admin",
                IsOwnerView = canSeeAllCompanyJobs
            }).ToList();
        }

        public async Task<SuccessResponse> CreateJob(CreateJobPostDto createjobdto)
        {
            var user = GetCurrentUserId();
            if (user == null) throw new Exception("User Null");

            var employerProfile = await _Ipermision.GetCurrentEmployerWithPermissionAsync(EmployerPermissions.PostJobs);

            if (employerProfile == null)
                return new SuccessResponse { Message = "You do not have permission to post jobs." };
            if (employerProfile.CompanyId == null)
                return new SuccessResponse { Message = "Employer profile or linked company not found" };

            var company = await _db.Companies.FindAsync(employerProfile.CompanyId);
            if (company == null || !company.IsVerified)
                return new SuccessResponse { Message = "Your company must be verified to post jobs." };

            var jobPost = new JobPost
            {
                Title = createjobdto.Title,
                Description = createjobdto.Description,
                Location = createjobdto.Location,
                JobType = createjobdto.JobType,
                ExperienceLevel = createjobdto.ExperienceLevel,
                SalaryMin = createjobdto.MinSalary,
                SalaryMax = createjobdto.MaxSalary,
                Currency = string.IsNullOrWhiteSpace(createjobdto.Currency) ? "USD" : createjobdto.Currency,
                Requirements = createjobdto.Requirements,
                Responsibilities = createjobdto.Responsibilities,
                CompanyId = employerProfile.CompanyId.Value,
                PostedByEmployerId = employerProfile.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                ExpirationDate = createjobdto.ExpirationDate ?? DateTime.UtcNow.AddDays(30)
            };

            _db.JobPosts.Add(jobPost);
            await _db.SaveChangesAsync();

            await _activity.LogAsync(jobPost.CompanyId, employerProfile, null, ActivityType.JobCreated,
             (a, _) => $"{a} posted the job \"{jobPost.Title}\".", refId: jobPost.Id);

            return new SuccessResponse
            {
                Message = "Job Added successfully."
            };
        }

        public async Task<SuccessResponse> DeleteJob(int id)
        {
            var employerProfile = await _Ipermision.GetCurrentEmployerWithPermissionAsync(EmployerPermissions.PostJobs)
                ?? throw new UnauthorizedAccessException("Employer does not have access.");

            // company check is part of the query
            var job = await _db.JobPosts
                    .FirstOrDefaultAsync(j => j.Id == id && j.CompanyId == employerProfile.CompanyId)
                    ?? throw new KeyNotFoundException("Job post not found.");

            if (!CanManageJob(employerProfile, job))
                throw new UnauthorizedAccessException("Only the job creator or a recruiter/admin can delete this job.");
            var title = job.Title;
            await _activity.LogAsync(job.CompanyId, employerProfile, null, ActivityType.JobDeleted,
                (a, _) => $"{a} deleted the job \"{title}\".");
            _db.JobPosts.Remove(job);
            await _db.SaveChangesAsync();

            return new SuccessResponse { Message = "Deleted Successfully" };
        }

        public async Task<SuccessResponse> UpdateJob(UpdateJobDto dto, int id)
        {
            var employer = await _Ipermision.GetCurrentEmployerWithPermissionAsync(EmployerPermissions.PostJobs)
                ?? throw new UnauthorizedAccessException("Employer does not have access.");

            var job = await _db.JobPosts
                   .FirstOrDefaultAsync(j => j.Id == id && j.CompanyId == employer.CompanyId)
                   ?? throw new KeyNotFoundException("Job post not found.");

            if (!CanManageJob(employer, job))
                throw new UnauthorizedAccessException("Only the job creator or a recruiter/admin can edit this job.");

            if (!string.IsNullOrWhiteSpace(dto.Title))
                job.Title = dto.Title;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                job.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Location))
                job.Location = dto.Location;
            if (!string.IsNullOrWhiteSpace(dto.Responsibilities))
                job.Responsibilities = dto.Responsibilities;
            if (!string.IsNullOrWhiteSpace(dto.Requirements))
                job.Requirements = dto.Requirements;

            if (dto.MinSalary.HasValue)
                job.SalaryMin = dto.MinSalary.Value;

            if (dto.MaxSalary.HasValue)
                job.SalaryMax = dto.MaxSalary.Value;

            if (dto.JobType.HasValue)
                job.JobType = dto.JobType.Value;

            if (dto.ExperienceLevel.HasValue)
                job.ExperienceLevel = dto.ExperienceLevel.Value;

            if (dto.IsActive.HasValue)
                job.IsActive = dto.IsActive.Value;
            var wasActive = job.IsActive;                       // before the "if (!string.IsNullOrWhiteSpace...)" block

            // ... your existing field assignments stay as they are ...

            var statusChanged = dto.IsActive.HasValue && dto.IsActive.Value != wasActive;
            var contentChanged = _db.Entry(job).Properties
                .Any(p => p.IsModified && p.Metadata.Name != nameof(JobPost.IsActive));

            if (contentChanged)
                await _activity.LogAsync(job.CompanyId, employer, null, ActivityType.JobUpdated,
                    (a, _) => $"{a} updated the job \"{job.Title}\".", refId: job.Id);

            if (statusChanged)
                await _activity.LogAsync(job.CompanyId, employer, null,
                    job.IsActive ? ActivityType.JobReopened : ActivityType.JobClosed,
                    (a, _) => job.IsActive
                        ? $"{a} reopened the job \"{job.Title}\"."
                        : $"{a} closed the job \"{job.Title}\".",
                    refId: job.Id);


            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "Job post updated successfully."
            };
        }

        private bool CanManageJob(EmployerProfile employer, JobPost job)
        {
            if (employer.CompanyId == null || job.CompanyId != employer.CompanyId)
                return false; // different company, never allowed

            var isCreator = job.PostedByEmployerId == employer.Id;
            var isRecruiterOrAbove = employer.Role is EmployerCompanyRole.Recruiter
                                                    or EmployerCompanyRole.Admin
                                                    or EmployerCompanyRole.Owner;

            return isCreator || isRecruiterOrAbove;
        }

        // ───────────────────────── SEARCH ─────────────────────────
        public async Task<List<SearchResponseDto>> Search(JobFilterDto jobfiltereddto)
        {
            var query = _db.JobPosts.AsQueryable();

            if (!string.IsNullOrEmpty(jobfiltereddto.Keyword))
                query = query.Where(j => j.Title.Contains(jobfiltereddto.Keyword));

            if (jobfiltereddto.PostedWithinDays.HasValue)
            {
                var date = DateTime.UtcNow.AddDays(-jobfiltereddto.PostedWithinDays.Value);
                query = query.Where(j => j.CreatedAt >= date);
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Select(j => new SearchResponseDto
                {
                    Id = j.Id,
                    logo = j.Company != null && j.Company.Logo != null ? j.Company.Logo.ImageUrl : null,
                    Title = j.Title,
                    Description = j.Description,
                    Location = j.Location
                })
                .ToListAsync();
        }

        public async Task<List<SearchResDto>> SearchJobsAsync(GlobalSearchDto dto)
        {
            var query = _db.JobPosts.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(dto.Keyword))
            {
                var keyword = dto.Keyword.Trim().ToLower();
                query = query.Where(j =>
                    j.Title.ToLower().Contains(keyword) ||
                    (j.Company != null && j.Company.CompanyName.ToLower().Contains(keyword)));
            }
            if (dto.IndustryId.HasValue)
                query = query.Where(j => j.Company.IndustryId == dto.IndustryId);

            if (!string.IsNullOrWhiteSpace(dto.EmploymentType))
            {
                if (!Enum.TryParse<JobType>(dto.EmploymentType, true, out var jobType)
                    || !Enum.IsDefined(typeof(JobType), jobType))
                    return new List<SearchResDto>();

                query = query.Where(j => j.JobType == jobType);
            }

            if (!string.IsNullOrWhiteSpace(dto.ExperienceLevel))
            {
                if (!Enum.TryParse<ExperienceLevel>(dto.ExperienceLevel, true, out var level)
                    || !Enum.IsDefined(typeof(ExperienceLevel), level))
                    return new List<SearchResDto>();

                query = query.Where(j => j.ExperienceLevel == level);
            }

            if (dto.PostedWithinDays.HasValue)
            {
                var date = DateTime.UtcNow.AddDays(-dto.PostedWithinDays.Value);
                query = query.Where(j => j.CreatedAt >= date);
            }

            if (!string.IsNullOrWhiteSpace(dto.Location))
            {
                var location = dto.Location.Trim().ToLower();
                query = query.Where(j => j.Location != null && j.Location.ToLower().Contains(location));
            }

            return await query
                .OrderByDescending(j => j.CreatedAt)
                .Take(50)
                .Select(j => new SearchResDto
                {
                    Id = j.Id,
                    logo = j.Company != null && j.Company.Logo != null ? j.Company.Logo.ImageUrl : null,
                    Title = j.Title,
                    Description = j.Description,
                    Location = j.Location,
                    ExperienceLevel = j.ExperienceLevel,
                    JobType = j.JobType,
                    MaxSalary = j.SalaryMax,
                    MinSalary = j.SalaryMin,
                    IndustryId = j.Company.IndustryId,
                    Industry = j.Company.Industry != null ? j.Company.Industry.Name : null,
                })
                .ToListAsync();
        }







        private static string CalculateAgeText(DateTime startDate, DateTime endDate)
        {
            var timeSpan = endDate - startDate;
            if (timeSpan.TotalDays < 1) return "Today";
            if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays}d ago";
            if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)}m ago";
            return $"{(int)(timeSpan.TotalDays / 365)}y ago";
        }

        // ───────────────────────── GET JOB (EMPLOYER VIEW) ─────────────────────────
        public async Task<ReturnedJobDto> GetJobById(int id)
        {
            var userId = GetCurrentUserId();
            var me = await _db.EmployerProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (me == null || me.CompanyId == null)
                throw new UnauthorizedAccessException("You are not in a company.");

            // company check is part of the query: other companies' jobs are "not found"
            var job = await _db.JobPosts
                .Include(x => x.Company)
                    .ThenInclude(x => x.Logo)
                .Include(x => x.PostedByEmployer).ThenInclude(x => x.User)
                .Include(x => x.JobApplications)
                .FirstOrDefaultAsync(j => j.Id == id && j.CompanyId == me.CompanyId)
                ?? throw new KeyNotFoundException("Job not found");

            // Role based (always correct after ownership transfer / removal):
            // job creator OR current Owner/Admin of this company
            bool isOwnerView = job.PostedByEmployerId == me.Id
                               || me.Role == EmployerCompanyRole.Owner
                               || me.Role == EmployerCompanyRole.Admin;

            return new ReturnedJobDto
            {
                Id = job.Id,
                Logo = job.Company?.Logo?.ImageUrl,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                MinSalary = job.SalaryMin,
                MaxSalary = job.SalaryMax,
                EmploymentType = job.JobType.ToString(),
                ExprianceLevel = job.ExperienceLevel.ToString(),
                Requirements = job.Requirements,
                Responsibilities = job.Responsibilities,
                Status = job.IsActive ? "Active" : "Inactive",

                CreatedAt = job.CreatedAt,
                PostedAge = GetTimeAgo(job.CreatedAt),

                CompanyName = job.Company?.CompanyName ?? string.Empty,
                CompanyAge = job.Company != null ? GetCompanyAge(job.Company.CreatedAt) : string.Empty,

                TotalApplications = job.JobApplications?.Count ?? 0,
                PostedByEmployerId = job.PostedByEmployerId.ToString(),
                PostedByName = job.PostedByEmployer?.User?.FullName ?? "Company Admin",
                IsOwnerView = isOwnerView
            };
        }

        public async Task<JobDetailsForJobseekerDto> GetJobDetailsForJobseeker(int jobId)
        {
            var userId = GetCurrentUserId();
            var job = await _db.JobPosts
                .Include(j => j.Company)
                    .ThenInclude(c => c.Logo)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                throw new KeyNotFoundException("Job post not found.");

            var seekerProfile = await _db.JobSeekers
                .FirstOrDefaultAsync(s => s.UserId == userId);
            bool hasApplied = false;
            int? applicationId = null;
            string? applicationStatus = null;

            if (seekerProfile != null)
            {
                var existingApplication = await _db.JobApplications
                    .FirstOrDefaultAsync(a => a.JobPostId == jobId && a.JobSeekerId == seekerProfile.Id);

                if (existingApplication != null)
                {
                    hasApplied = true;
                    applicationId = existingApplication.Id;
                    applicationStatus = existingApplication.Status.ToString();
                }
            }

            return new JobDetailsForJobseekerDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                MinSalary = job.SalaryMin,
                MaxSalary = job.SalaryMax,
                EmploymentType = job.JobType.ToString(),
                ExperienceLevel = job.ExperienceLevel.ToString(),

                CreatedAt = job.CreatedAt,
                PostedAge = GetTimeAgo(job.CreatedAt),
                Requirements = job.Requirements,
                Responsibilities = job.Responsibilities,
                CompanyId = job.CompanyId,
                CompanyName = job.Company?.CompanyName ?? string.Empty,
                CompanyLogo = job.Company?.Logo?.ImageUrl,
                CompanyWebsite = job.Company?.CompanyWebsite ?? string.Empty,

                HasApplied = hasApplied,
                ApplicationId = applicationId,
                ApplicationStatus = applicationStatus,
                IsActive = job.IsActive
            };
        }




        private string GetTimeAgo(DateTime date)
        {
            var span = DateTime.UtcNow - date;
            if (span.TotalDays >= 365) return $"{(int)(span.TotalDays / 365)} year(s) ago";
            if (span.TotalDays >= 30) return $"{(int)(span.TotalDays / 30)} month(s) ago";
            if (span.TotalDays >= 7) return $"{(int)(span.TotalDays / 7)} week(s) ago";
            if (span.TotalDays >= 1) return $"{(int)span.TotalDays} day(s) ago";
            if (span.TotalHours >= 1) return $"{(int)span.TotalHours} hour(s) ago";
            return "Just now";
        }

        private string GetCompanyAge(DateTime foundedDate)
        {
            var years = (DateTime.UtcNow - foundedDate).TotalDays / 365;
            return years < 1 ? "Less than a year old" : $"{(int)years} year(s) old";
        }
      

        public async Task<IEnumerable<JobDto>> AllJobs()
        {
            return await _db.JobPosts
                .Include(x => x.Company)
                    .ThenInclude(x => x.Logo)
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    logo = j.Company != null && j.Company.Logo != null ? j.Company.Logo.ImageUrl : null,
                    Title = j.Title,
                    Description = j.Description,
                    Location = j.Location,
                    MinSalary = j.SalaryMin,
                    MaxSalary = j.SalaryMax
                })
                .ToListAsync();
        }









        public async Task<JobDto> GetJob(int id)
        {
            var job = await _db.JobPosts
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                throw new KeyNotFoundException("Job not found");

            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                Responsibilities = job.Responsibilities,
                Location = job.Location,
                MinSalary = job.SalaryMin,
                MaxSalary = job.SalaryMax
            };
        }

        public async Task<List<RecommendedJobsDto>> GetRecommendedJobs()
        {
            var userid = GetCurrentUserId();
            var seeker = await _db.JobSeekers
                .Include(s => s.Skills)
                .FirstOrDefaultAsync(s => s.UserId == userid);

            if (seeker == null)
                return new List<RecommendedJobsDto>();

            var activeJobs = await _db.JobPosts
                .Include(e => e.Company)
                    .ThenInclude(x => x.Logo)
                .Where(j => j.IsActive)
                .ToListAsync();

            var results = new List<RecommendedJobsDto>();

            foreach (var job in activeJobs)
            {
                int score = CalculateMatchScore(job, seeker.Skills);
                results.Add(new RecommendedJobsDto
                {
                    JobId = job.Id,
                    Title = job.Title,
                    logo = job.Company?.Logo?.ImageUrl,
                    CompanyName = job.Company?.CompanyName,
                    MatchScore = score,
                    Location = job.Location,
                    MinSalary = job.SalaryMin,
                    MaxSalary = job.SalaryMax
                });
            }

            return results
                .OrderByDescending(r => r.MatchScore)
                .Where(x => x.MatchScore >= 1)
                .Take(10)
                .ToList();
        }

        private int CalculateMatchScore(JobPost job, ICollection<Skill> skills)
        {
            if (skills == null || !skills.Any()) return 0;
            var requirements = job.Requirements?.ToLower() ?? "";
            int matched = skills.Count(x => requirements.Contains(x.Name.ToLower()));
            return (matched * 100) / skills.Count;
        }

        public async Task<SuccessResponse> SaveAlert(string keyword)
        {
            var userId = GetCurrentUserId();

            var alert = new SavedJobAlert
            {
                UserId = userId,
                Keyword = keyword,
                LastChecked = DateTime.UtcNow
            };

            _db.SavedJobAlerts.Add(alert);
            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "Alert Saved"
            };
        }

        public async Task<List<JobPost>> FindNewJobs(SavedJobAlert alert)
        {
            return await _db.JobPosts
                .Where(j => j.Title.Contains(alert.Keyword) && j.CreatedAt > alert.LastChecked)
                .ToListAsync();
        }
        public async Task<PagedResult<EmployerJobCardDto>?> GetEmployerJobsAsync(EmployerJobsFilterDto filter)
        {
            var employer = await _Ipermision.GetCurrentEmployerWithPermissionAsync(EmployerPermissions.PostJobs);
            if (employer == null || employer.CompanyId == null)
                return null;

            var canSeeAllCompanyJobs = _Ipermision.HasPermission(employer, EmployerPermissions.ManageEmployerRoles);
            var now = DateTime.UtcNow;

            var query = _db.JobPosts
                        .AsNoTracking()
                        .Where(j => j.CompanyId == employer.CompanyId);

            if (!canSeeAllCompanyJobs)
                query = query.Where(j => j.PostedByEmployerId == employer.Id);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim();
                query = query.Where(j => j.Title.Contains(keyword) || j.Location.Contains(keyword));
            }

            if (filter.Status.HasValue)
            {
                query = filter.Status.Value switch
                {
                    JobStatus.Active => query.Where(j => j.IsActive && j.ExpirationDate > now),
                    JobStatus.Expired => query.Where(j => j.IsActive && j.ExpirationDate <= now),
                    JobStatus.Closed => query.Where(j => !j.IsActive),
                    _ => query
                };
            }

            if (filter.EmploymentType.HasValue)
                query = query.Where(j => j.JobType == filter.EmploymentType.Value);

            if (filter.ExperienceLevel.HasValue)
                query = query.Where(j => j.ExperienceLevel == filter.ExperienceLevel.Value);

            query = filter.SortBy switch
            {
                "oldest" => query.OrderBy(j => j.CreatedAt),
                "applicants_desc" => query.OrderByDescending(j => j.JobApplications.Count()),
                _ => query.OrderByDescending(j => j.CreatedAt)
            };

            var pageNumber = Math.Max(filter.PageNumber, 1);
            var pageSize = Math.Clamp(filter.PageSize, 1, 50);

            var totalCount = await query.CountAsync();

            // Database part: only the card fields
            var rawJobs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    LogoUrl = j.Company.Logo != null ? j.Company.Logo.ImageUrl : null,
                    j.Location,
                    j.JobType,
                    j.ExperienceLevel,
                    j.IsActive,
                    j.SalaryMin,
                    j.SalaryMax,
                    j.Currency,
                    j.CreatedAt,
                    j.ExpirationDate,
                    ApplicationsCount = j.JobApplications.Count(),
                    PostedByName = j.PostedByEmployer != null && j.PostedByEmployer.User != null
                        ? j.PostedByEmployer.User.FullName
                        : null
                })
                .ToListAsync();

            // Memory part: things SQL can't do
            var items = rawJobs.Select(j => new EmployerJobCardDto
            {
                Id = j.Id,
                Title = j.Title,
                Logo = j.LogoUrl,
                Location = j.Location,
                EmploymentType = j.JobType,
                ExperienceLevel = j.ExperienceLevel,
                Status = !j.IsActive ? JobStatus.Closed
                       : j.ExpirationDate <= now ? JobStatus.Expired
                       : JobStatus.Active,
                MinSalary = j.SalaryMin,
                MaxSalary = j.SalaryMax,
                Currency = j.Currency,
                CreatedAt = j.CreatedAt,
                ExpirationDate = j.ExpirationDate,
                PostedAge = CalculateAgeText(j.CreatedAt, now),
                TotalApplications = j.ApplicationsCount,
                PostedByName = j.PostedByName ?? "Company Admin",
                IsOwnerView = canSeeAllCompanyJobs
            }).ToList();

            return new PagedResult<EmployerJobCardDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<PagedResult<JobCardDto>> GetActiveJobsAsync(JobsFilterDto filter)
        {
            var now = DateTime.UtcNow;

            var query = _db.JobPosts
                .AsNoTracking()
                .Where(j => j.IsActive && j.ExpirationDate > now);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim();
                query = query.Where(j => j.Title.Contains(keyword) || j.Location.Contains(keyword));
            }

            if (filter.EmploymentType.HasValue)
                query = query.Where(j => j.JobType == filter.EmploymentType.Value);

            if (filter.ExperienceLevel.HasValue)
                query = query.Where(j => j.ExperienceLevel == filter.ExperienceLevel.Value);

            if (!string.IsNullOrWhiteSpace(filter.Industry))
            {
                var slug = filter.Industry.Trim().ToLower();
                query = query.Where(j => j.Company.Industry != null
                                      && j.Company.Industry.Slug.ToLower() == slug);
            }

            var since = filter.PostedWithin?.Trim().ToLower() switch
            {
                "day" => now.AddDays(-1),
                "3days" => now.AddDays(-3),
                "week" => now.AddDays(-7),
                "month" => now.AddMonths(-1),
                _ => (DateTime?)null
            };
            if (since.HasValue)
                query = query.Where(j => j.CreatedAt >= since.Value);

            query = filter.SortBy == "oldest"
                ? query.OrderBy(j => j.CreatedAt)
                : query.OrderByDescending(j => j.CreatedAt);

            var pageNumber = Math.Max(filter.PageNumber, 1);
            var pageSize = Math.Clamp(filter.PageSize, 1, 50);
            var totalCount = await query.CountAsync();

            var rawJobs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    LogoUrl = j.Company.Logo != null ? j.Company.Logo.ImageUrl : null,
                    CompanyName = j.Company.CompanyName,
                    IndustryName = j.Company.Industry != null ? j.Company.Industry.Name : null,
                    j.Location,
                    j.JobType,
                    j.ExperienceLevel,
                    j.SalaryMin,
                    j.SalaryMax,
                    j.Currency,
                    j.CreatedAt,
                    j.ExpirationDate
                })
                .ToListAsync();

            var items = rawJobs.Select(j => new JobCardDto
            {
                Id = j.Id,
                Title = j.Title,
                Logo = j.LogoUrl,
                CompanyName = j.CompanyName,
                IndustryName = j.IndustryName ?? string.Empty,
                Location = j.Location,
                EmploymentType = j.JobType,
                ExperienceLevel = j.ExperienceLevel,
                MinSalary = j.SalaryMin,
                MaxSalary = j.SalaryMax,
                Currency = j.Currency,
                CreatedAt = j.CreatedAt,
                ExpirationDate = j.ExpirationDate,
                PostedAge = CalculateAgeText(j.CreatedAt, now)
            }).ToList();

            return new PagedResult<JobCardDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<Application.DTOs.JobPost.JobDTO.RecordsDTO.RecordPagedResult<Application.DTOs.JobPost.JobDTO.RecordsDTO.RecommendedJobDto>?> RecommendAsync(int page, int size, CancellationToken ct)
        {

            var userId = _currentuser.UserId;              
            if (string.IsNullOrEmpty(userId)) return null;

            page = Math.Max(page, 0);
            size = Math.Clamp(size, 1, 50);

            var seeker = await _db.JobSeekers
             .AsNoTracking()
             .Where(s => s.UserId == userId)
             .Select(s => new { s.Id, Skills = s.Skills.Select(k => k.Name).ToList() })
             .FirstOrDefaultAsync(ct);

            if (seeker == null) return null;

            var skills = seeker.Skills
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var now = DateTime.UtcNow;

            // 2) Bounded candidate query: active, not expired, not already applied
            var candidates = await _db.JobPosts
                .AsNoTracking()
                .Where(j => j.IsActive && j.ExpirationDate > now)
                .Where(j => !_db.JobApplications.Any(a => a.JobPostId == j.Id && a.JobSeekerId == seeker.Id))
                .OrderByDescending(j => j.CreatedAt)
                .Take(300)
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    LogoUrl = j.Company.Logo != null ? j.Company.Logo.ImageUrl : null,
                    CompanyName = j.Company.CompanyName,
                    j.Location,
                    j.JobType,
                    j.SalaryMin,
                    j.SalaryMax,
                    j.Currency,
                    j.CreatedAt,
                    j.Requirements
                })
                .ToListAsync(ct);


            var matchers = skills
        .Select(s => (Name: s, Rx: new Regex(
            $@"(?<![A-Za-z0-9]){Regex.Escape(s)}(?![A-Za-z0-9])",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(50))))
        .ToList();

            var scored = candidates.Select(j =>
            {
                var text = j.Requirements ?? string.Empty;
                var matched = matchers.Where(m => m.Rx.IsMatch(text)).Select(m => m.Name).ToList();
                int score = matchers.Count == 0 ? 0 : matched.Count * 100 / matchers.Count;

                return new RecommendedJobDto(
                    j.Id,
                    j.Title,
                    j.LogoUrl, 
                    j.CompanyName,
                    j.Location,
                    j.JobType.ToString(),
                    j.SalaryMin,
                    j.SalaryMax,
                    j.Currency,
                    score,
                    matched,
                    j.CreatedAt);
            });

            var ordered = (matchers.Count == 0
           ? scored
           : scored.Where(r => r.MatchScore > 0))
          .OrderByDescending(r => r.MatchScore)
           .ThenByDescending(r => r.PostedAtUtc)
          .ToList();

            var items = ordered.Skip(page * size).Take(size).ToList();
            return new Application.DTOs.JobPost.JobDTO.RecordsDTO.RecordPagedResult<Application.DTOs.JobPost.JobDTO.RecordsDTO.RecommendedJobDto>(items, page, size, ordered.Count);
        }

        public async Task<IReadOnlyList<LatestJobDto>> GetLatestJobsAsync(int count = 6, CancellationToken ct = default)
        {
            count = Math.Clamp(count, 1, 20);

            var cacheKey = $"latest_jobs_{count}";
            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<LatestJobDto>? cached) && cached is not null)
                return cached;

            var jobs = await _db.JobPosts.Include(c=>c.Company).ThenInclude(x=>x.Logo)
                .AsNoTracking()
                .Where(j => j.IsActive)
                .OrderByDescending(j => j.CreatedAt)
                .Take(count)
                .Select(j => new LatestJobDto(
                    j.Id,
                    j.Title,
                    j.Company.CompanyName,
                    j.Company.Logo.ImageUrl,
                    j.Location,
                    j.JobType.ToString(),
                    j.SalaryMin,
                    j.SalaryMax,
                    j.CreatedAt))
                .ToListAsync(ct);

            _cache.Set(cacheKey, jobs, CacheDuration);
            return jobs;
        }
    }
}