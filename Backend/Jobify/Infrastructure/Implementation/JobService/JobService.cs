using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.industry;
using Application.DTOs.JobPost;
using Application.DTOs.Pagenation;
using Application.DTOs.RecommendedJobbs;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.Job;
using Domain.Entities;
using Domain.Entities.LooksUp;
using Infrastructure.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.JobService
{
    public class JobService : IJobService
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpaccessor;
        public JobService(AppDBContext db, IHttpContextAccessor http)
        {
            _db = db;
            _httpaccessor = http;
        }

        private string? GetCurrentUserId()
        {
            var user = _httpaccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }


        public async Task<SuccessResponse> CreateJob(CreateJob createjobdto)
        {
            var user = GetCurrentUserId();
            if (user == null) throw new Exception("User Null");
            var employerProfile = await _db.EmployerProfiles.Include(c => c.Company)
                .FirstOrDefaultAsync(e => e.UserId == user);
            
            var Job = new JobPost
            {

                Title = createjobdto.Title,
                Description = createjobdto.Description,
                Location = createjobdto.Location,
                MinSalary = createjobdto.MinSalary,
                MaxSalary = createjobdto.MaxSalary,
                IndustryId = createjobdto.IndustryId,
                EmploymentTypeId = createjobdto.EmploymentTypeId,
                StatusId = createjobdto.StatusId,// default Draft
                CompanyId = employerProfile.Company.Id,
                WorkModeId = createjobdto.WorkModeId,
                CreatedByUserId = user,
                EducationOrExperience = createjobdto.EducationOrExperience,
                Requirements = createjobdto.Responsibilities,
                Responsibilities = createjobdto.Requirements,
                isActive=true,
                CreatedAt = DateTime.UtcNow
            };


            await _db.JobPosts.AddAsync(Job);
            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "Job Added successfully."
            };

        }

        public async Task<ReturnedJobDto> GetJobById(int id)
        {
            var job = await _db.JobPosts
                .Include(x=>x.Company).ThenInclude(x=>x.Logo)
             .Include(j => j.WorkMode)
             .Include(j => j.Industry)
             .Include(j => j.EmploymentType)
             .Include(j => j.Status)
             .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                throw new KeyNotFoundException("Job not found");

            return new ReturnedJobDto
            {
                Id = job.Id,
                logo = job.Company != null && job.Company.Logo != null
                ? job.Company.Logo.ImageUrl
                : null,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                Requirements = job.Requirements,
                Responsibilities = job.Responsibilities,
                EducationOrExperience = job.EducationOrExperience,
                WorkMode = job.WorkMode.Name,
                Industry = job.Industry.IndustryName,
                EmploymentType = job.EmploymentType.TypeName,
                Status = job.Status.StatusName
            };
        }


        public async Task<IEnumerable<JobDto>> AllJobs()
        {
            return await _db.JobPosts
                .Include(x=>x.Company).ThenInclude(x=>x.Logo)
                .Select(j => new JobDto
                {
                    
                    Id = j.Id,
                    logo = j.Company != null && j.Company.Logo != null
                ? j.Company.Logo.ImageUrl
                : null,
                    Title = j.Title,
                    Description = j.Description,
                    Location = j.Location,
                    MinSalary = j.MinSalary,
                    MaxSalary = j.MaxSalary,
                    IndustryId = j.IndustryId,
                    EmploymentTypeId = j.EmploymentTypeId,
                    StatusId = j.StatusId
                    ,
                    WorkModeId = j.WorkModeId
                })
                .ToListAsync();
        }

        public async Task<SuccessResponse> UpdateJob(UpdateJobDto updatejob, int id)
        {
            var user = GetCurrentUserId();
            if (user == null) throw new Exception("User Null");

            var employer = await _db.EmployerProfiles
                            .FirstOrDefaultAsync(e => e.UserId == user);

            if (employer?.Company == null)
                throw new Exception("Company not found");

            var job = await _db.JobPosts
                .FirstOrDefaultAsync(j =>
                    j.Id == id &&
                    j.CompanyId == employer.Company.Id); 

            job.Title = updatejob.Title;
            job.Description = updatejob.Description;
            job.Location = updatejob.Location;
            job.MaxSalary = updatejob.MaxSalary;
            job.MinSalary = updatejob.MinSalary;
            job.IndustryId = updatejob.IndustryId;
            job.WorkModeId = updatejob.WorkModeId;
            job.EmploymentTypeId = updatejob.EmploymentTypeId;


            await _db.SaveChangesAsync();
            return new SuccessResponse
            {
                Message = "Update Succefully"
            };

        }

        public async Task<SuccessResponse> DeleteJob(int id)
        {
            var user = GetCurrentUserId();
            if (user == null) throw new Exception("User Null");

            var employerProfile = await _db.EmployerProfiles
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.UserId == user);

            if (employerProfile?.Company == null)
                throw new Exception("Company not found");

            var job = await _db.JobPosts
                .FirstOrDefaultAsync(j =>
                    j.Id == id &&
                    j.CompanyId == employerProfile.Company.Id); 

            if (job == null)
                throw new Exception("Job not found");

            _db.JobPosts.Remove(job);
            await _db.SaveChangesAsync();

            return new SuccessResponse { Message = "Deleted Successfully" };
        }

        

        public async Task<IEnumerable<ReturnedJobDto>> AllJobsOfEmployer()
        {
             var userid = GetCurrentUserId();

    var employerProfile = await _db.EmployerProfiles
        .Include(e => e.Company).ThenInclude(x=>x.Logo)
        .FirstOrDefaultAsync(x => x.UserId == userid);

    if (employerProfile?.Company == null)
        throw new Exception("Company not found");
            var jobs = await _db.JobPosts
     .Where(j => j.CompanyId == employerProfile.Company.Id) // ✅ changed
     .Select(j => new ReturnedJobDto
     {
         Id = j.Id,
         Title = j.Title,
         logo = j.Company != null && j.Company.Logo != null
                ? j.Company.Logo.ImageUrl
                : null,
         Description = j.Description,
         Location = j.Location,
         MinSalary = j.MinSalary,
         MaxSalary = j.MaxSalary,
         Industry = j.Industry.IndustryName,
         EmploymentType = j.EmploymentType.TypeName,
         Status = j.Status.StatusName,
         WorkMode = j.WorkMode.Name
     })
     .ToListAsync();

            return jobs;

        }

        public async Task<IndustryDto?> JobWithIndustry(int id)
        {
            var result = await _db.JobPosts
                .Where(j => j.IndustryId == id)  // filter by industry id
                .GroupBy(j => j.IndustryId)
                .Select(g => new IndustryDto
                {
                    industryName = _db.JobIndustries
                                       .Where(i => i.Id == g.Key)
                                       .Select(i => i.IndustryName)
                                       .FirstOrDefault(),
                    industrycount = g.Count()
                })
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<IEnumerable<IndustryDto>> Industries()
        {
            var result = await _db.JobIndustries
                    .GroupJoin(
                        _db.JobPosts,
                        industry => industry.Id,       // Key from Industry
                        job => job.IndustryId,         // Key from JobPost
                        (industry, jobs) => new IndustryDto
                        {
                            industryName = industry.IndustryName,
                            industrycount = jobs.Count()  // Count jobs for this industry
                        }
                    )
                    .ToListAsync();

            return result; ;

        }

        public async Task<PageDto<JobDto>> FilteredJobs(
           JobFilterDto jobfiltereddto,
           int pagesize = 10,
           int pagenumber = 1)
        {
            var query = _db.JobPosts.AsQueryable();

            if (!string.IsNullOrEmpty(jobfiltereddto.Keyword))
                query = query.Where(j => j.Title.Contains(jobfiltereddto.Keyword));

            if (jobfiltereddto.IndustryIds != null && jobfiltereddto.IndustryIds.Any())
                query = query.Where(x => jobfiltereddto.IndustryIds.Contains(x.IndustryId));

            if (jobfiltereddto.EmploymentTypeIds != null && jobfiltereddto.EmploymentTypeIds.Any())
                query = query.Where(x => jobfiltereddto.EmploymentTypeIds.Contains(x.EmploymentTypeId));

            if (jobfiltereddto.WorkModeIds != null && jobfiltereddto.WorkModeIds.Any())
                query = query.Where(x => jobfiltereddto.WorkModeIds.Contains(x.WorkModeId));


            if (jobfiltereddto.PostedWithinDays.HasValue)
            {
                var date = DateTime.UtcNow.AddDays(-jobfiltereddto.PostedWithinDays.Value);
                query = query.Where(j => j.CreatedAt >= date);
            }

            var totalJobs = await query.CountAsync();

            var jobs = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pagenumber - 1) * pagesize)
                .Take(pagesize)
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    logo = j.Company != null && j.Company.Logo != null
                ? j.Company.Logo.ImageUrl
                : null,
                    Title = j.Title,
                    IndustryId = j.IndustryId,
                    EmploymentTypeId = j.EmploymentTypeId,
                    Description = j.Description,
                    MaxSalary = j.MaxSalary,
                    MinSalary = j.MinSalary,
                    Location = j.Location,
                    WorkModeId = j.WorkModeId
                })
                .ToListAsync();

            return new PageDto<JobDto>
            {
                currentpage = pagenumber,
                pagesize = pagesize,
                totaljobs = totalJobs,
                items = jobs
            };
        }

        public async Task<JobDto> GetJob(int id)
        {

            var job = await _db.JobPosts
             .Include(j => j.WorkMode)
             .Include(j => j.Industry)
             .Include(j => j.EmploymentType)
             .Include(j => j.Status)
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
                EducationOrExperience = job.EducationOrExperience,
                Location = job.Location,
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,

                WorkModeId = job.WorkModeId,
                IndustryId = job.IndustryId,
                EmploymentTypeId = job.IndustryId,
                StatusId = job.StatusId
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

            // Change this include
            var acticejobs = await _db.JobPosts
                .Include(x=>x.EmploymentType)
                .Include(x=>x.Industry).Include(x=>x.Status)
                .Include(x=>x.WorkMode)
                .Include(e => e.Company).ThenInclude(x=>x.Logo) // ✅ changed from EmployerProfile
                .ToListAsync();
            var results = new List<RecommendedJobsDto>();

            foreach (var job in acticejobs)
            {
                ICollection<Skill> skills = seeker.Skills;
                int score = CalculateMatchScore(job, skills);
                results.Add(new RecommendedJobsDto
                {
                    JobId = job.Id,
                    Title = job.Title,
                    logo = job.Company != null && job.Company.Logo != null
                ? job.Company.Logo.ImageUrl
                : null,
                    CompanyName = job.Company?.CompanyName, // ✅ changed
                    MatchScore = score,
                    Location = job.Location,
                    MinSalary = job.MinSalary,
                    MaxSalary = job.MaxSalary,
                    IndustryId = job.Industry.Id,
                    EmploymentTypeId = job.EmploymentType.Id,
                    WorkModeId = job.WorkMode.Id,
                    StatusId = job.Status.Id
                });
            }
            return results
                  .OrderByDescending(r => r.MatchScore)
                  .Where(x => x.MatchScore >= 1)
                  .Take(10).ToList();

        }
        private int CalculateMatchScore(JobPost job, ICollection<Skill> skills)
        {
            if (skills.Count() == 0) return 0;
            var requirements = job.Requirements?.ToLower() ?? "";
            int matched = skills.Count(x => requirements.Contains(x.Name.ToLower()));
            return (matched * 100) / skills.Count();
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

            var jobs = await _db.JobPosts
              .Where(j => j.Title.Contains(alert.Keyword))
               .Where(j => j.CreatedAt > alert.LastChecked)
                 .ToListAsync();

            return jobs;

        }

       
    public async Task<List<OpeningJobs>> AvalibleJobs()
        {


            var userid = GetCurrentUserId();

            var employerProfile = await _db.EmployerProfiles
                .Include(e => e.Company)
                .FirstOrDefaultAsync(x => x.UserId == userid);

            if (employerProfile?.Company == null)
                throw new Exception("Company not found");
            var jobs = await _db.JobPosts
     .Where(j => j.CompanyId == employerProfile.Company.Id && j.Status.StatusName == "Published"
             && j.isActive == true) // ✅ changed
     .Select(j => new OpeningJobs
     {
         Id = j.Id,
         Title = j.Title,
         Description = j.Description,
         Location = j.Location,
         MinSalary = j.MinSalary,
         MaxSalary = j.MaxSalary,
         IndustryId = j.Industry.Id,
         EmploymentTypeId = j.EmploymentType.Id,
         StatusId = j.Status.Id,
         WorkModeId = j.WorkMode.Id
     })
     .ToListAsync();

            return jobs;




        }

        public async Task<List<SearchResponseDto>> Search(string keyword)
        {
            var jobs = await _db.JobPosts.Include(x=>x.Company)
        .Where(j => j.Title.ToLower().Contains(keyword.ToLower()) ||
                    j.Description.ToLower().Contains(keyword.ToLower()))
        .Select(j => new  SearchResponseDto{
            Id=j.Id,
            Title=j.Title,
            logo=j.Company.Logo.ImageUrl,
            Location=j.Location,
            IndustryId = j.Industry.Id,
            EmploymentTypeId = j.EmploymentType.Id,
            StatusId = j.Status.Id,
            WorkModeId = j.WorkMode.Id,
            data=j.CreatedAt
            
        })
        .ToListAsync();


            return jobs;
        }
    }
}