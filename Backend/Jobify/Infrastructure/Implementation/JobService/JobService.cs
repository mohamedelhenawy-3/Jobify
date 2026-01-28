using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.industry;
using Application.DTOs.JobPost;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.Job;
using Domain.Entities;
using Infrastructure.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.JobService
{
    public class JobService :IJobService
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpaccessor;
        public JobService(AppDBContext db,IHttpContextAccessor http)
        {
            _db = db;
            _httpaccessor = http;
        }

        private string? GetCurrentUserId()
        {
            var user = _httpaccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }


        public async   Task<SuccessResponse> CreateJob(CreateJob createjobdto)
        {
            var user = GetCurrentUserId();
            if (user == null) throw new Exception("User Null");
            var employerProfile = await _db.EmployerProfiles
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
                StatusId = createjobdto.StatusId,
                EmployerProfileId = employerProfile.Id,
                WorkModeId=createjobdto.WorkModeId,
                CreatedByUserId = user,
                CreatedAt = DateTime.UtcNow
            };
            await _db.JobPosts.AddAsync(Job);
            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "Job Added successfully."
            };

        }

        public async Task<JobDto> GetJobById(int id)
        {
            var job =  _db.JobPosts.FirstOrDefault(x => x.Id == id);
            if (job == null) return null;
            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                StatusId = job.StatusId,
                IndustryId = job.IndustryId,
                EmploymentTypeId = job.EmploymentTypeId
                ,WorkModeId=job.WorkModeId
            };
            
        }

        public async Task<IEnumerable<JobDto>> AllJobs()
        {
            return await _db.JobPosts
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Location = j.Location,
                    MinSalary = j.MinSalary,
                    MaxSalary = j.MaxSalary,
                    IndustryId = j.IndustryId,
                    EmploymentTypeId = j.EmploymentTypeId,
                    StatusId = j.StatusId
                    ,WorkModeId=j.WorkModeId
                })
                .ToListAsync();
        }

        public async Task<SuccessResponse> UpdateJob(UpdateJobDto updatejob, int id)
        {
            var user = GetCurrentUserId();
            if (user == null) throw new Exception("User Null");

            var employer = await _db.EmployerProfiles
                            .FirstOrDefaultAsync(e => e.UserId == user);
            var job = await _db.JobPosts
                   .FirstOrDefaultAsync(j =>
                       j.Id == id &&
                       j.EmployerProfileId == employer.Id);

            job.Title = updatejob.Title;
            job.Location = updatejob.Location;
            job.MaxSalary = updatejob.MaxSalary;
            job.MinSalary = updatejob.MinSalary;
            job.IndustryId = updatejob.IndustryId;
            job.StatusId = updatejob.StatusId;
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

            var employer = await _db.EmployerProfiles
                            .FirstOrDefaultAsync(e => e.UserId == user);
            var job = await _db.JobPosts
                   .FirstOrDefaultAsync(j =>
                       j.Id == id &&
                       j.EmployerProfileId == employer.Id);

            _db.JobPosts.Remove(job);
            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "Deleted Succefully"
            };

        }

        public async Task<IEnumerable<JobDto>> AllJobsOfEmployer(int employerId)
        {
            var jobs = await _db.JobPosts
                  .Where(j => j.EmployerProfileId == employerId)
                  .Select(j => new JobDto
                  {
                      Id = j.Id,
                      Title = j.Title,
                      Description = j.Description,
                      Location = j.Location,
                      MinSalary = j.MinSalary,
                      MaxSalary = j.MaxSalary,
                      IndustryId = j.IndustryId,
                      EmploymentTypeId = j.EmploymentTypeId,
                      StatusId = j.StatusId,
                      WorkModeId=j.WorkModeId
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

        public async  Task<IEnumerable<IndustryDto>> Industries()
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

        public async Task<IEnumerable<JobDto>> FilteredJobs(JobFilterDto jobfiltereddto)
        {
            var query =_db.JobPosts.AsQueryable();

            if (!string.IsNullOrEmpty(jobfiltereddto.Keyword))
            {
                query = query.Where(j => j.Title.Contains(jobfiltereddto.Keyword));
            }
            if (jobfiltereddto.IndustryId.HasValue)
            {
                query = query.Where(x => x.IndustryId == jobfiltereddto.IndustryId);
            }
            if (jobfiltereddto.EmploymentTypeId.HasValue)
            {
                query = query.Where(x => x.EmploymentTypeId == jobfiltereddto.EmploymentTypeId);
            }
            if (jobfiltereddto.WorkModeId.HasValue)
            {
                query = query.Where(x => x.WorkModeId == jobfiltereddto.WorkModeId);
            }
            if (jobfiltereddto.PostedWithinDays.HasValue)
            {
                var date = DateTime.UtcNow.AddDays(-jobfiltereddto.PostedWithinDays.Value);
                query = query.Where(j => j.CreatedAt >= date);
            }

            return await query
              .Select(j => new JobDto { Id = j.Id, Title = j.Title,
                  IndustryId=j.IndustryId,
                  EmploymentTypeId=j.EmploymentTypeId,
                  Description=j.Description,
              MaxSalary=j.MaxSalary,
              MinSalary=j.MinSalary,
              Location=j.Location})
              .ToListAsync();  
        }
    }
}
