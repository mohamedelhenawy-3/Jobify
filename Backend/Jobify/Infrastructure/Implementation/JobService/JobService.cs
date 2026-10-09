using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.industry;
using Application.DTOs.JobPost;
using Application.DTOs.JobPost.JobDTO;
using Application.DTOs.Pagenation;
using Application.DTOs.RecommendedJobbs;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.Job;
using Application.Interfaces.Permisions;
using Application.SearchFeature.Dtos;
using Domain.Entities;
using Domain.Entities.LooksUp;
using Domain.Entities.LooksUp.EmployerPermisions;
using Domain.Entities.LooksUp.JobEnum;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace Infrastructure.Implementation.JobService
{
    public class JobService : IJobService
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpaccessor;
        private readonly IEmployerPermissionService _Ipermision;

        public JobService(AppDBContext db, IHttpContextAccessor http, IEmployerPermissionService Ipermision)
        {
            _db = db;
            _httpaccessor = http;
            _Ipermision = Ipermision;
        }

        private string? GetCurrentUserId()
        {
            var user = _httpaccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public async
            Task<List<EmployerJobItemDto>> GetMyJobsAsync()
        {
            var userId = GetCurrentUserId();
            var me = await _db.EmployerProfiles
               .Include(e => e.User) // Explicitly load User navigation property if FullName lives on User
               .FirstOrDefaultAsync(x => x.UserId == userId);
        

            if (me == null)
                return new List<EmployerJobItemDto>();

            var query = _db.JobPosts.AsNoTracking();

            bool seesWholeCompany = me.CompanyId != null &&
                (me.Role == EmployerCompanyRole.Owner || me.Role == EmployerCompanyRole.Admin);

            query = seesWholeCompany
                ? query.Where(j => j.CompanyId == me.CompanyId)
                : query.Where(j => j.PostedByEmployerId == me.Id);

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
                    PostedBy = seesWholeCompany ? j.PostedByEmployer.User.FullName : null
                })
                .ToListAsync();

            return jobs;
        }
        public async Task<IEnumerable<ReturnedJobDto>?> GetEmployerJobsAsync()
        {
            var employer = await _Ipermision.GetCurrentEmployerWithPermissionAsync(EmployerPermissions.PostJobs);
            if (employer == null)
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
                Requirements=j.Requirements,
                Responsibilities=j.Responsibilities,
                CompanyName = j.CompanyName,
                CompanyAge = CalculateAgeText(j.CompanyCreatedAt, now),
                TotalApplications = j.ApplicationsCount,
                PostedByEmployerId = j.PostedByUserId,
                PostedByName = j.PostedByName ?? "Company Admin",
                IsOwnerView = canSeeAllCompanyJobs
            }).ToList();
        }
    
        //public async Task<PagedResult<ReturnedJobDto>> SearchJobs(JobsFilterDto filter)
        //{
        //    var query = _db.JobPosts
        //            .Include(j => j.Company)
        //                .ThenInclude(c => c.Logo)
        //            .Where(j => j.IsActive)
        //            .AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(filter.Keyword))
        //        query = query.Where(j => j.Title.Contains(filter.Keyword)
        //                               || j.Description.Contains(filter.Keyword));

        //    if (!string.IsNullOrWhiteSpace(filter.Location))
        //        query = query.Where(j => j.Location.Contains(filter.Location));

        //    // Parse string -> enum BEFORE the query, then compare enum == enum
        //    if (!string.IsNullOrWhiteSpace(filter.EmploymentType)
        //        && Enum.TryParse<JobType>(filter.EmploymentType, out var jobType))
        //    {
        //        query = query.Where(j => j.JobType == jobType);
        //    }

        //    if (!string.IsNullOrWhiteSpace(filter.ExperienceLevel)
        //        && Enum.TryParse<ExperienceLevel>(filter.ExperienceLevel, out var expLevel))
        //    {
        //        query = query.Where(j => j.ExperienceLevel == expLevel);
        //    }

        //    if (filter.MinSalary.HasValue)
        //        query = query.Where(j => j.SalaryMax == null || j.SalaryMax >= filter.MinSalary);

        //    if (filter.MaxSalary.HasValue)
        //        query = query.Where(j => j.SalaryMin == null || j.SalaryMin <= filter.MaxSalary);

        //    query = filter.SortBy switch
        //    {
        //        "salary_desc" => query.OrderByDescending(j => j.SalaryMax),
        //        "salary_asc" => query.OrderBy(j => j.SalaryMin),
        //        _ => query.OrderByDescending(j => j.CreatedAt)
        //    };

        //    var totalCount = await query.CountAsync();

        //    var jobs = await query
        //        .Skip((filter.PageNumber - 1) * filter.PageSize)
        //        .Take(filter.PageSize)
        //        .Select(j => new ReturnedJobDto
        //        {
        //            Id = j.Id,
        //            Logo = j.Company.Logo != null ? j.Company.Logo.ImageUrl : null,
        //            Title = j.Title,
        //            Description = j.Description,
        //            Location = j.Location,
        //            MinSalary = j.SalaryMin,
        //            MaxSalary = j.SalaryMax,
        //            EmploymentType = j.JobType.ToString(),      // ✅ fine here — this is a projection, not a filter
        //            ExprianceLevel = j.ExperienceLevel.ToString(), // ✅ fine here too
        //            Status = j.IsActive ? "Active" : "Inactive",
        //            CreatedAt = j.CreatedAt,
        //            CompanyName = j.Company.CompanyName
        //        })
        //        .ToListAsync();

        //    return new PagedResult<ReturnedJobDto>
        //    {
        //        Items = jobs,
        //        TotalCount = totalCount,
        //        PageNumber = filter.PageNumber,
        //        PageSize = filter.PageSize
        //    };
        //}
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

            return new SuccessResponse
            {
                Message = "Job Added successfully."
            };
        }

        public async Task<SuccessResponse> DeleteJob(int id)
        {
            var employerProfile = await _Ipermision.GetCurrentEmployerWithPermissionAsync(EmployerPermissions.PostJobs);
            if (employerProfile == null)
                throw new Exception("Employer not Have Access");

            var job = await _db.JobPosts
                    .FirstOrDefaultAsync(j => j.Id == id && j.CompanyId == employerProfile.CompanyId);

            if (!CanManageJob(employerProfile, job))
                throw new Exception("Only the job creator or a recruiter-Admin can delete this job.");


            _db.JobPosts.Remove(job);
            await _db.SaveChangesAsync();

            return new SuccessResponse { Message = "Deleted Successfully" };
        }

        public async Task<SuccessResponse> UpdateJob(UpdateJobDto dto, int id)
        {
            var employer = await _Ipermision.GetCurrentEmployerWithPermissionAsync(EmployerPermissions.PostJobs);
            if (employer == null)
                throw new Exception("Employer not Have Access");



            var job = await _db.JobPosts
                   .FirstOrDefaultAsync(j => j.Id == id && j.CompanyId == employer.CompanyId);

            if (job == null)
                throw new KeyNotFoundException("Job post not found.");

            if (!CanManageJob(employer, job))
                throw new Exception("Only the job creator or a recruiter can edit this job.");

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

            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "Job post updated successfully."
            };
        }

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
                    MaxSalary=j.SalaryMax,
                    MinSalary=j.SalaryMin
                })
                .ToListAsync();
        }
        private bool CanManageJob(EmployerProfile employer, JobPost job)
        {
            if (job.CompanyId != employer.CompanyId)
                return false; // different company, never allowed

            var isCreator = job.PostedByEmployerId == employer.Id;
            var isRecruiterOrAbove = employer.Role is EmployerCompanyRole.Recruiter
                                                    or EmployerCompanyRole.Admin
                                                    or EmployerCompanyRole.Owner;

            return isCreator || isRecruiterOrAbove;
        }

        private static string CalculateAgeText(DateTime startDate, DateTime endDate)
        {
            var timeSpan = endDate - startDate;
            if (timeSpan.TotalDays < 1) return "Today";
            if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays}d ago";
            if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)}m ago";
            return $"{(int)(timeSpan.TotalDays / 365)}y ago";
        }
        public async Task<ReturnedJobDto> GetJobById(int id)
        {

            var userId = GetCurrentUserId();
            var currentEmployerProfile = await _db.EmployerProfiles
               .Include(e => e.User) // Explicitly load User navigation property if FullName lives on User
               .FirstOrDefaultAsync(x => x.UserId == userId);

            if (currentEmployerProfile == null)
                return null;
            var job = await _db.JobPosts
                .Include(x => x.Company)
                    .ThenInclude(x => x.Logo)
                .Include(x => x.PostedByEmployer).ThenInclude(x => x.User) // navigation to EmployerProfile who posted it
                .Include(x => x.JobApplications)      // to count applications
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                throw new KeyNotFoundException("Job not found");

            // Get the current logged-in employer's profile (if any)

            // Ownership check: job creator OR company owner
            bool isJobCreator = currentEmployerProfile != null
                && job.PostedByEmployerId == currentEmployerProfile.Id;

            bool isCompanyOwner = currentEmployerProfile != null
                && job.Company.CreatedByEmployerId == currentEmployerProfile.Id;

            bool isOwnerView = isJobCreator || isCompanyOwner;

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
                Requirements=job.Requirements,
                Responsibilities=job.Responsibilities,
                Status = job.IsActive ? "Active" : "Inactive",

                CreatedAt = job.CreatedAt,
                PostedAge = GetTimeAgo(job.CreatedAt),

                CompanyName = job.Company?.CompanyName ?? string.Empty,
                CompanyAge = job.Company != null ? GetCompanyAge(job.Company.CreatedAt) : string.Empty,

                TotalApplications = job.JobApplications?.Count ?? 0,
                PostedByEmployerId = job.PostedByEmployerId.ToString(),
                PostedByName = job.PostedByEmployer.User.FullName,
                IsOwnerView = isOwnerView
            };
        }
        public async  Task<JobDetailsForJobseekerDto> GetJobDetailsForJobseeker(int jobId)
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

            // 4. Construct Response DTO
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
                Requirements=job.Requirements,
                Responsibilities=job.Responsibilities,
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



        public async  Task<PagedResult<EmployerJobCardDto>?> GetEmployerJobsAsync(EmployerJobsFilterDto filter)
        {
            var employer = await _Ipermision.GetCurrentEmployerWithPermissionAsync(EmployerPermissions.PostJobs);
            if (employer == null)
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

        public async  Task<PagedResult<JobCardDto>> GetActiveJobsAsync(JobsFilterDto filter)
        {
            var now = DateTime.UtcNow;

            var query = _db.JobPosts.Include(x=>x.Company)
                                      .ThenInclude(x=>x.Logo)
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
    }
}